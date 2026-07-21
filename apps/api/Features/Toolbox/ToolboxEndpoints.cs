using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Toolbox;
using QAHub.Api.Infrastructure.Data;
using QAHub.Api.Infrastructure.Security;
namespace QAHub.Api.Features.Toolbox;
public sealed record ValidateSqlRequest(string Statement);
public sealed record SaveSqlQueryRequest(Guid? ProductId,string Name,string Statement);
public sealed record SavedSqlQueryResponse(Guid Id,Guid? ProductId,string Name,string Statement,string CreatedBy,DateTimeOffset CreatedAtUtc);
public static class ToolboxEndpoints
{
 public static IEndpointRouteBuilder MapToolboxEndpoints(this IEndpointRouteBuilder endpoints){var group=endpoints.MapGroup("/api/v1/toolbox").WithTags("Toolbox").RequireAuthorization(AuthorizationPolicies.ToolboxUse);group.MapPost("/sql/validate",(ValidateSqlRequest request)=>Results.Ok(SqlSafetyPolicy.Validate(request.Statement)));group.MapGet("/sql/library",GetLibrary);group.MapPost("/sql/library",SaveQuery);return endpoints;}
 private static async Task<IResult> GetLibrary(QAHubDbContext db,Guid? productId,CancellationToken ct){var query=db.SavedSqlQueries.AsNoTracking().AsQueryable();if(productId.HasValue)query=query.Where(x=>x.ProductId==productId);return Results.Ok(await query.OrderByDescending(x=>x.CreatedAtUtc).Select(x=>new SavedSqlQueryResponse(x.Id,x.ProductId,x.Name,x.Statement,x.CreatedBy,x.CreatedAtUtc)).ToListAsync(ct));}
 private static async Task<IResult> SaveQuery(SaveSqlQueryRequest request,QAHubDbContext db,ClaimsPrincipal user,CancellationToken ct){var validation=SqlSafetyPolicy.Validate(request.Statement);if(!validation.IsSafe)return Results.ValidationProblem(new Dictionary<string,string[]>{{"statement",validation.Violations.ToArray()}});if(string.IsNullOrWhiteSpace(request.Name))return Results.ValidationProblem(new Dictionary<string,string[]>{{"name",["Name is required."]}});if(request.ProductId.HasValue&&!await db.Products.AnyAsync(x=>x.Id==request.ProductId,ct))return Results.ValidationProblem(new Dictionary<string,string[]>{{"productId",["Product does not exist."]}});var item=new SavedSqlQuery(request.ProductId,request.Name,validation.NormalizedStatement,user.Identity?.Name??"unknown");db.SavedSqlQueries.Add(item);await db.SaveChangesAsync(ct);return Results.Created($"/api/v1/toolbox/sql/library/{item.Id}",new SavedSqlQueryResponse(item.Id,item.ProductId,item.Name,item.Statement,item.CreatedBy,item.CreatedAtUtc));}
}
