using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Execution;
using QAHub.Api.Domain.TestDesign;
using QAHub.Api.Infrastructure.Data;
using QAHub.Api.Infrastructure.Security;

namespace QAHub.Api.Features.Execution;

public static class ExecutionEndpoints
{
    public static IEndpointRouteBuilder MapExecutionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/execution").WithTags("Execution").RequireAuthorization(AuthorizationPolicies.ProductAccess);
        group.MapGet("/builds", GetBuilds);
        group.MapGet("/candidates", GetCandidates);
        group.MapPost("/builds", CreateBuild);
        group.MapGet("/cycles", GetCycles);
        group.MapPost("/cycles", CreateCycle);
        group.MapGet("/cycles/{id:guid}", GetCycle);
        group.MapPost("/cycles/{id:guid}/start", (Guid id, QAHubDbContext db, CancellationToken ct) => Transition(id, "start", db, ct));
        group.MapPost("/cycles/{id:guid}/complete", (Guid id, QAHubDbContext db, CancellationToken ct) => Transition(id, "complete", db, ct));
        group.MapPost("/cycles/{id:guid}/cancel", (Guid id, QAHubDbContext db, CancellationToken ct) => Transition(id, "cancel", db, ct));
        group.MapGet("/cycles/{id:guid}/report", DownloadReport);
        group.MapPost("/cycle-items/{id:guid}/attempts", Execute);
        group.MapPost("/attempts/{id:guid}/evidence", UploadEvidence);
        group.MapGet("/attempts/{id:guid}/evidence", GetEvidence);
        group.MapGet("/attempts/{attemptId:guid}/evidence/{evidenceId:guid}", DownloadEvidence);
        return endpoints;
    }

    private static async Task<IResult> GetBuilds(QAHubDbContext db, Guid? productId, CancellationToken ct)
    {
        var query = db.ProductBuilds.AsNoTracking().AsQueryable();
        if (productId.HasValue) query = query.Where(x => x.ProductId == productId);
        var data = await query.OrderByDescending(x => x.CreatedAtUtc).Select(x => new BuildResponse(x.Id, x.ProductId, x.Version, x.CreatedAtUtc)).ToListAsync(ct);
        return Results.Ok(data);
    }

    private static async Task<IResult> GetCandidates(Guid productId, QAHubDbContext db, CancellationToken ct)
    {
        var data = await db.TestCaseVersions.AsNoTracking()
            .Where(v => v.Status == TestCaseStatus.Approved || v.Status == TestCaseStatus.Active)
            .Join(db.TestCases.Where(tc => tc.ProductId == productId), v => v.TestCaseId, tc => tc.Id,
                (v, tc) => new ExecutionCandidateResponse(v.Id, tc.Id, tc.Code, v.Title, v.VersionNumber, v.Status))
            .OrderBy(x => x.Code).ThenByDescending(x => x.VersionNumber).ToListAsync(ct);
        return Results.Ok(data);
    }

    private static async Task<IResult> CreateBuild(CreateBuildRequest request, QAHubDbContext db, CancellationToken ct)
    {
        var version = request.Version?.Trim();
        if (string.IsNullOrWhiteSpace(version) || version.Length > 100) return Validation("version", "Build version is required and must not exceed 100 characters.");
        if (!await db.Products.AnyAsync(x => x.Id == request.ProductId && x.IsActive, ct)) return Validation("productId", "An active product is required.");
        if (await db.ProductBuilds.AnyAsync(x => x.ProductId == request.ProductId && x.Version == version, ct)) return Results.Conflict(new { message = "Build version already exists." });
        var build = new ProductBuild(request.ProductId, version);
        db.ProductBuilds.Add(build);
        await db.SaveChangesAsync(ct);
        return Results.Created($"/api/v1/execution/builds/{build.Id}", new BuildResponse(build.Id, build.ProductId, build.Version, build.CreatedAtUtc));
    }

    private static async Task<IResult> GetCycles(QAHubDbContext db, Guid? productId, CancellationToken ct)
    {
        var query = CycleQuery(db);
        if (productId.HasValue) query = query.Where(x => x.ProductId == productId);
        return Results.Ok((await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct)).Select(x => Map(x)));
    }

    private static async Task<IResult> CreateCycle(CreateCycleRequest request, QAHubDbContext db, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 250) return Validation("name", "Cycle name is required and must not exceed 250 characters.");
        var environmentValid = await db.ProductEnvironments.AnyAsync(x => x.Id == request.EnvironmentId && x.ProductId == request.ProductId && x.IsActive, ct);
        if (!environmentValid) return Validation("environmentId", "Environment must be active and belong to the selected product.");
        if (request.BuildId.HasValue && !await db.ProductBuilds.AnyAsync(x => x.Id == request.BuildId && x.ProductId == request.ProductId, ct)) return Validation("buildId", "Build must belong to the selected product.");
        var ids = (request.TestCaseVersionIds ?? []).Distinct().ToArray();
        if (ids.Length == 0) return Validation("testCaseVersionIds", "Select at least one test case version.");
        var validCount = await db.TestCaseVersions.CountAsync(v => ids.Contains(v.Id) && db.TestCases.Any(tc => tc.Id == v.TestCaseId && tc.ProductId == request.ProductId) && (v.Status == TestCaseStatus.Approved || v.Status == TestCaseStatus.Active), ct);
        if (validCount != ids.Length) return Validation("testCaseVersionIds", "All test cases must belong to the product and be Approved or Active.");
        var cycle = new TestCycle(request.ProductId, request.EnvironmentId, request.BuildId, request.Name, request.Assignee ?? "");
        foreach (var id in ids) cycle.Items.Add(new TestCycleItem(cycle.Id, id, request.Assignee ?? ""));
        db.TestCycles.Add(cycle);
        await db.SaveChangesAsync(ct);
        return Results.Created($"/api/v1/execution/cycles/{cycle.Id}", await MapWithMetadata(cycle, db, ct));
    }

    private static async Task<IResult> GetCycle(Guid id, QAHubDbContext db, CancellationToken ct)
    {
        var cycle = await CycleQuery(db).SingleOrDefaultAsync(x => x.Id == id, ct);
        return cycle is null ? Results.NotFound() : Results.Ok(await MapWithMetadata(cycle, db, ct));
    }

    private static async Task<IResult> Transition(Guid id, string action, QAHubDbContext db, CancellationToken ct)
    {
        var cycle = await CycleQuery(db).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (cycle is null) return Results.NotFound();
        try
        {
            if (action == "start") cycle.Start(); else if (action == "complete") cycle.Complete(); else cycle.Cancel();
            await db.SaveChangesAsync(ct);
            return Results.Ok(await MapWithMetadata(cycle, db, ct));
        }
        catch (InvalidOperationException ex) { return Results.Conflict(new { message = ex.Message }); }
    }

    private static async Task<IResult> Execute(Guid id, ExecuteItemRequest request, ClaimsPrincipal user, QAHubDbContext db, CancellationToken ct)
    {
        var item = await db.TestCycleItems.Include(x => x.Attempts).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return Results.NotFound();
        if (!await db.TestCycles.AnyAsync(x => x.Id == item.TestCycleId && x.Status == TestCycleStatus.InProgress, ct)) return Results.Conflict(new { message = "The cycle must be in progress before recording results." });
        try
        {
            var attempt = new TestRunAttempt(id, item.Attempts.Count + 1, request.Result, request.ActualResult ?? "", request.Evidence ?? "", request.Reason ?? "", user.Identity?.Name ?? "unknown");
            item.Attempts.Add(attempt);
            await db.SaveChangesAsync(ct);
            return Results.Ok(Map(attempt));
        }
        catch (ArgumentException ex) { return Validation("result", ex.Message); }
    }

    private static async Task<IResult> UploadEvidence(Guid id, CreateEvidenceRequest request, ClaimsPrincipal user, QAHubDbContext db, CancellationToken ct)
    {
        var attempt = await db.TestRunAttempts.Include(x => x.EvidenceFiles).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (attempt is null) return Results.NotFound();
        try
        {
            var content = Convert.FromBase64String(request.ContentBase64 ?? "");
            var evidence = new TestRunEvidence(id, request.FileName ?? "", request.ContentType ?? "application/octet-stream", content, user.Identity?.Name ?? "unknown");
            attempt.EvidenceFiles.Add(evidence);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/v1/execution/attempts/{id}/evidence/{evidence.Id}", Map(evidence));
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException) { return Validation("file", ex.Message); }
    }

    private static async Task<IResult> GetEvidence(Guid id, QAHubDbContext db, CancellationToken ct) =>
        Results.Ok((await db.TestRunEvidenceFiles.AsNoTracking().Where(x => x.TestRunAttemptId == id).OrderByDescending(x => x.UploadedAtUtc).ToListAsync(ct)).Select(Map));

    private static async Task<IResult> DownloadEvidence(Guid attemptId, Guid evidenceId, QAHubDbContext db, CancellationToken ct)
    {
        var file = await db.TestRunEvidenceFiles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == evidenceId && x.TestRunAttemptId == attemptId, ct);
        return file is null ? Results.NotFound() : Results.File(file.Content, file.ContentType, file.FileName);
    }

    private static async Task<IResult> DownloadReport(Guid id, QAHubDbContext db, CancellationToken ct)
    {
        var cycle = await CycleQuery(db).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (cycle is null) return Results.NotFound();
        var model = await MapWithMetadata(cycle, db, ct);
        var csv = new StringBuilder("Test Case,Title,Version,Assignee,Result,Attempts,Executed By,Executed At\r\n");
        foreach (var item in model.Items) { var latest = item.Attempts.Count == 0 ? null : item.Attempts[^1]; csv.AppendLine(string.Join(',', Csv(item.TestCaseCode), Csv(item.Title), item.VersionNumber, Csv(item.Assignee), item.Result, item.AttemptCount, Csv(latest?.ExecutedBy ?? ""), latest?.ExecutedAtUtc.ToString("O") ?? "")); }
        return Results.File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(), "text/csv", $"{SafeName(cycle.Name)}-report.csv");
    }

    private static IQueryable<TestCycle> CycleQuery(QAHubDbContext db) => db.TestCycles.Include(x => x.Items).ThenInclude(x => x.Attempts).ThenInclude(x => x.EvidenceFiles).AsSplitQuery();
    private static async Task<CycleResponse> MapWithMetadata(TestCycle cycle, QAHubDbContext db, CancellationToken ct)
    {
        var ids = cycle.Items.Select(x => x.TestCaseVersionId).ToArray();
        var metadata = await db.TestCaseVersions.Where(x => ids.Contains(x.Id)).Join(db.TestCases, v => v.TestCaseId, tc => tc.Id, (v, tc) => new { v.Id, tc.Code, v.Title, v.VersionNumber }).ToDictionaryAsync(x => x.Id, ct);
        return Map(cycle, metadata.ToDictionary(x => x.Key, x => (x.Value.Code, x.Value.Title, x.Value.VersionNumber)));
    }
    private static CycleResponse Map(TestCycle cycle, IReadOnlyDictionary<Guid, (string Code, string Title, int Version)>? metadata = null)
    {
        var latest = cycle.Items.Select(i => i.Attempts.OrderBy(a => a.AttemptNumber).LastOrDefault()).ToList();
        var items = cycle.Items.Select(i => { var m = metadata?.GetValueOrDefault(i.TestCaseVersionId) ?? ("", "", 0); var a = i.Attempts.OrderBy(x => x.AttemptNumber).LastOrDefault(); return new CycleItemResponse(i.Id, i.TestCaseVersionId, m.Code, m.Title, m.Version, i.Assignee, a?.Result ?? ExecutionResult.NotRun, i.Attempts.Count, i.Attempts.OrderBy(x => x.AttemptNumber).Select(Map).ToList()); }).ToList();
        return new CycleResponse(cycle.Id, cycle.ProductId, cycle.EnvironmentId, cycle.BuildId, cycle.Name, cycle.Assignee, cycle.Status, cycle.CreatedAtUtc, cycle.StartedAtUtc, cycle.CompletedAtUtc, items, items.Count, latest.Count(a => a is not null), latest.Count(a => a?.Result == ExecutionResult.Passed), latest.Count(a => a?.Result == ExecutionResult.Failed), latest.Count(a => a?.Result == ExecutionResult.Blocked), latest.Count(a => a?.Result == ExecutionResult.Skipped));
    }
    private static AttemptResponse Map(TestRunAttempt x) => new(x.Id, x.AttemptNumber, x.Result, x.ActualResult, x.Evidence, x.Reason, x.ExecutedBy, x.ExecutedAtUtc, x.EvidenceFiles.Select(Map).ToList());
    private static EvidenceResponse Map(TestRunEvidence x) => new(x.Id, x.FileName, x.ContentType, x.SizeBytes, x.UploadedBy, x.UploadedAtUtc);
    private static IResult Validation(string key, string message) => Results.ValidationProblem(new Dictionary<string, string[]> { [key] = [message] });
    private static string Csv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
    private static string SafeName(string value) => string.Concat(value.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '-' : c));
}
