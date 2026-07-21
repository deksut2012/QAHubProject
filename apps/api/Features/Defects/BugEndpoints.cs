using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Defects;
using QAHub.Api.Domain.Execution;
using QAHub.Api.Infrastructure.Data;
using QAHub.Api.Infrastructure.Security;

namespace QAHub.Api.Features.Defects;
public static class BugEndpoints
{
 public static IEndpointRouteBuilder MapBugEndpoints(this IEndpointRouteBuilder endpoints){var g=endpoints.MapGroup("/api/v1/bugs").WithTags("Bugs").RequireAuthorization(AuthorizationPolicies.ProductAccess);g.MapGet("/",GetAll);g.MapGet("/{id:guid}",Get);g.MapPost("/",Create);g.MapPost("/{id:guid}/transition",Transition);g.MapPost("/{id:guid}/comments",AddComment);g.MapPost("/{id:guid}/evidence",UploadEvidence);g.MapGet("/{id:guid}/evidence/{evidenceId:guid}",DownloadEvidence);return endpoints;}
 private static async Task<IResult> GetAll(QAHubDbContext db,Guid? productId,BugStatus? status,BugSeverity? severity,string? search,CancellationToken ct){var q=db.Bugs.AsNoTracking().Include(x=>x.RunLinks).Include(x=>x.History).AsSplitQuery().AsQueryable();if(productId.HasValue)q=q.Where(x=>x.ProductId==productId);if(status.HasValue)q=q.Where(x=>x.Status==status);if(severity.HasValue)q=q.Where(x=>x.Severity==severity);if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>x.Code.Contains(search)||x.Title.Contains(search));return Results.Ok((await q.OrderByDescending(x=>x.CreatedAtUtc).ToListAsync(ct)).Select(Map));}
 private static async Task<IResult> Get(Guid id,QAHubDbContext db,CancellationToken ct){var x=await Query(db).SingleOrDefaultAsync(x=>x.Id==id,ct);return x is null?Results.NotFound():Results.Ok(Map(x));}
 private static async Task<IResult> Create(CreateBugRequest r,ClaimsPrincipal user,QAHubDbContext db,CancellationToken ct)
 {
  if(string.IsNullOrWhiteSpace(r.Title)||string.IsNullOrWhiteSpace(r.StepsToReproduce)||string.IsNullOrWhiteSpace(r.ActualResult))return Validation("bug","Title, steps to reproduce and actual result are required.");
  if(!await db.Products.AnyAsync(x=>x.Id==r.ProductId&&x.IsActive,ct))return Validation("productId","An active product is required.");
  var attemptIds=(r.TestRunAttemptIds??[]).Distinct().ToArray();
  if(attemptIds.Length>0){var valid=await db.TestRunAttempts.CountAsync(a=>attemptIds.Contains(a.Id)&&a.Result==ExecutionResult.Failed&&db.TestCycleItems.Any(i=>i.Id==a.TestCycleItemId&&db.TestCycles.Any(c=>c.Id==i.TestCycleId&&c.ProductId==r.ProductId)),ct);if(valid!=attemptIds.Length)return Validation("testRunAttemptIds","Every linked attempt must be a failed run from the selected product.");}
  var code=string.IsNullOrWhiteSpace(r.Code)?$"BUG-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}":r.Code.Trim().ToUpperInvariant();if(await db.Bugs.AnyAsync(x=>x.ProductId==r.ProductId&&x.Code==code,ct))return Results.Conflict(new{message="Bug code already exists."});
  try{var bug=new Bug(r.ProductId,code,r.Title,r.Description??"",r.StepsToReproduce,r.ExpectedResult??"",r.ActualResult,r.Severity,r.Priority,user.Identity?.Name??"unknown");if(!string.IsNullOrWhiteSpace(r.Assignee))bug.Assign(r.Assignee);foreach(var id in attemptIds)bug.RunLinks.Add(new BugRunLink(bug.Id,id));db.Bugs.Add(bug);await db.SaveChangesAsync(ct);return Results.Created($"/api/v1/bugs/{bug.Id}",Map(bug));}catch(ArgumentException ex){return Validation("bug",ex.Message);}
 }
 private static async Task<IResult> Transition(Guid id,TransitionBugRequest r,ClaimsPrincipal user,QAHubDbContext db,CancellationToken ct)
 {
  var bug=await Query(db).SingleOrDefaultAsync(x=>x.Id==id,ct);if(bug is null)return Results.NotFound();if(!string.IsNullOrWhiteSpace(r.Assignee))bug.Assign(r.Assignee);
  if(r.FixBuildId.HasValue&&!await db.ProductBuilds.AnyAsync(x=>x.Id==r.FixBuildId&&x.ProductId==bug.ProductId,ct))return Validation("fixBuildId","Fix build must belong to the bug product.");
  if(r.CanonicalBugId.HasValue&&(r.CanonicalBugId==bug.Id||!await db.Bugs.AnyAsync(x=>x.Id==r.CanonicalBugId&&x.ProductId==bug.ProductId,ct)))return Validation("canonicalBugId","Canonical bug must be another bug in the same product.");
  if(r.VerificationAttemptId.HasValue&&!await db.TestRunAttempts.AnyAsync(a=>a.Id==r.VerificationAttemptId&&a.Result==ExecutionResult.Passed&&db.TestCycleItems.Any(i=>i.Id==a.TestCycleItemId&&db.TestCycles.Any(c=>c.Id==i.TestCycleId&&c.ProductId==bug.ProductId)),ct))return Validation("verificationAttemptId","Verification attempt must be a passed run from the bug product.");
  try{bug.TransitionTo(r.Status,user.Identity?.Name??"unknown",r.Reason??"",r.FixBuildId,r.CanonicalBugId,r.VerificationAttemptId);await db.SaveChangesAsync(ct);return Results.Ok(Map(bug));}catch(InvalidOperationException ex){return Results.Conflict(new{message=ex.Message});}
 }
 private static async Task<IResult> AddComment(Guid id,CreateBugCommentRequest r,ClaimsPrincipal user,QAHubDbContext db,CancellationToken ct){var bug=await Query(db).SingleOrDefaultAsync(x=>x.Id==id,ct);if(bug is null)return Results.NotFound();try{var comment=new BugComment(id,user.Identity?.Name??"unknown",r.Body??"");bug.Comments.Add(comment);await db.SaveChangesAsync(ct);return Results.Created($"/api/v1/bugs/{id}/comments/{comment.Id}",new BugCommentResponse(comment.Id,comment.AuthorId,comment.Body,comment.CreatedAtUtc));}catch(ArgumentException ex){return Validation("body",ex.Message);}}
 private static async Task<IResult> UploadEvidence(Guid id,CreateBugEvidenceRequest r,ClaimsPrincipal user,QAHubDbContext db,CancellationToken ct){var bug=await Query(db).SingleOrDefaultAsync(x=>x.Id==id,ct);if(bug is null)return Results.NotFound();try{var content=Convert.FromBase64String(r.ContentBase64??"");var file=new BugEvidence(id,r.FileName??"",r.ContentType??"",content,user.Identity?.Name??"unknown");bug.EvidenceFiles.Add(file);await db.SaveChangesAsync(ct);return Results.Created($"/api/v1/bugs/{id}/evidence/{file.Id}",Map(file));}catch(Exception ex)when(ex is ArgumentException or FormatException){return Validation("file",ex.Message);}}
 private static async Task<IResult> DownloadEvidence(Guid id,Guid evidenceId,QAHubDbContext db,CancellationToken ct){var file=await db.BugEvidenceFiles.AsNoTracking().SingleOrDefaultAsync(x=>x.BugId==id&&x.Id==evidenceId,ct);return file is null?Results.NotFound():Results.File(file.Content,file.ContentType,file.FileName);}
 private static IQueryable<Bug> Query(QAHubDbContext db)=>db.Bugs.Include(x=>x.RunLinks).Include(x=>x.History).Include(x=>x.Comments).Include(x=>x.EvidenceFiles).AsSplitQuery();
 private static BugResponse Map(Bug x)=>new(x.Id,x.ProductId,x.Code,x.Title,x.Description,x.StepsToReproduce,x.ExpectedResult,x.ActualResult,x.Severity,x.Priority,x.Status,x.Reporter,x.Assignee,x.FixBuildId,x.CanonicalBugId,x.VerificationAttemptId,x.CreatedAtUtc,x.UpdatedAtUtc,Math.Max(0,(int)(DateTimeOffset.UtcNow-(x.ClosedAtUtc??x.CreatedAtUtc)).TotalDays),x.RunLinks.Select(l=>l.TestRunAttemptId).ToList(),x.History.OrderBy(h=>h.ChangedAtUtc).Select(h=>new BugHistoryResponse(h.FromStatus,h.ToStatus,h.ActorId,h.Reason,h.ChangedAtUtc)).ToList(),x.Comments.OrderBy(c=>c.CreatedAtUtc).Select(c=>new BugCommentResponse(c.Id,c.AuthorId,c.Body,c.CreatedAtUtc)).ToList(),x.EvidenceFiles.OrderBy(e=>e.UploadedAtUtc).Select(Map).ToList());
 private static BugEvidenceResponse Map(BugEvidence x)=>new(x.Id,x.FileName,x.ContentType,x.SizeBytes,x.UploadedBy,x.UploadedAtUtc);
 private static IResult Validation(string key,string message)=>Results.ValidationProblem(new Dictionary<string,string[]>{{key,[message]}});
}
