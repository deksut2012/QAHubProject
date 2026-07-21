using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Defects;
using QAHub.Api.Domain.Execution;
using QAHub.Api.Infrastructure.Data;
using QAHub.Api.Infrastructure.Security;

namespace QAHub.Api.Features.Reporting;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/dashboard")
            .WithTags("Dashboard")
            .RequireAuthorization(AuthorizationPolicies.ProductAccess);
        group.MapGet("/", Get);
        group.MapGet("/report", Report);
        return endpoints;
    }

    private static async Task<IResult> Get(QAHubDbContext db, Guid? productId, CancellationToken ct) =>
        Results.Ok(await Build(db, productId, ct));

    private static async Task<IResult> Report(QAHubDbContext db, Guid? productId, CancellationToken ct)
    {
        var dashboard = await Build(db, productId, ct);
        var csv = new StringBuilder("Section,Metric,Value\r\n");
        Append(csv, "Summary", "Active Products", dashboard.ActiveProducts);
        Append(csv, "Summary", "Requirements", dashboard.Requirements);
        Append(csv, "Summary", "Requirement Coverage", $"{dashboard.RequirementCoverage}%");
        Append(csv, "Summary", "Test Cases", dashboard.TestCases);
        Append(csv, "Bugs", "Open", dashboard.OpenBugs);
        Append(csv, "Bugs", "Critical/High", dashboard.CriticalHighBugs);
        Append(csv, "Bugs", "SLA Breached", dashboard.BugAging.Breached);
        Append(csv, "Bugs", "Average Age Days", dashboard.BugAging.AverageAgeDays);
        Append(csv, "Execution", "Pass Rate", $"{dashboard.Execution.PassRate}%");
        Append(csv, "Releases", "Average Readiness", $"{dashboard.AverageReleaseReadiness}%");
        foreach (var release in dashboard.ReleaseReadiness)
            Append(csv, "Release Readiness", release.Name, $"{release.Score}% ({release.Completed}/{release.Required})");
        return Results.File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(), "text/csv", "qa-dashboard-report.csv");
    }

    private static void Append(StringBuilder csv, string section, string metric, object value) =>
        csv.AppendLine(string.Join(',', Csv(section), Csv(metric), Csv(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty)));

    private static string Csv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";

    private static async Task<DashboardResponse> Build(QAHubDbContext db, Guid? productId, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var products = db.Products.AsNoTracking().Where(x => x.IsActive);
        if (productId.HasValue) products = products.Where(x => x.Id == productId);

        var requirements = db.Requirements.AsNoTracking().AsQueryable();
        var testCases = db.TestCases.AsNoTracking().AsQueryable();
        var bugs = db.Bugs.AsNoTracking().AsQueryable();
        var cycles = db.TestCycles.AsNoTracking().Include(x => x.Items).ThenInclude(x => x.Attempts).AsSplitQuery().AsQueryable();
        var releases = db.Releases.AsNoTracking().Include(x => x.Checklist).AsSplitQuery().AsQueryable();
        if (productId.HasValue)
        {
            requirements = requirements.Where(x => x.ProductId == productId);
            testCases = testCases.Where(x => x.ProductId == productId);
            bugs = bugs.Where(x => x.ProductId == productId);
            cycles = cycles.Where(x => x.ProductId == productId);
            releases = releases.Where(x => x.ProductId == productId);
        }

        var requirementCount = await requirements.CountAsync(ct);
        var coveredRequirements = await requirements.CountAsync(r => db.TestCases.Any(tc => tc.RequirementId == r.Id), ct);
        var terminal = new[] { BugStatus.Closed, BugStatus.Rejected, BugStatus.Duplicate, BugStatus.CannotReproduce };
        var openBugs = await bugs.Where(x => !terminal.Contains(x.Status)).ToListAsync(ct);
        var cycleData = await cycles.ToListAsync(ct);
        var latest = cycleData.SelectMany(c => c.Items).Select(i => i.Attempts.OrderBy(a => a.AttemptNumber).LastOrDefault()).ToList();
        var executed = latest.Count(x => x is not null);
        var attempts = cycleData.SelectMany(c => c.Items).SelectMany(i => i.Attempts).Where(a => a.ExecutedAtUtc >= now.AddDays(-13)).ToList();
        var trend = Enumerable.Range(0, 14)
            .Select(offset => DateOnly.FromDateTime(now.UtcDateTime.Date.AddDays(offset - 13)))
            .Select(date => new TrendPoint(date,
                attempts.Count(a => DateOnly.FromDateTime(a.ExecutedAtUtc.UtcDateTime) == date && a.Result == ExecutionResult.Passed),
                attempts.Count(a => DateOnly.FromDateTime(a.ExecutedAtUtc.UtcDateTime) == date && a.Result == ExecutionResult.Failed),
                attempts.Count(a => DateOnly.FromDateTime(a.ExecutedAtUtc.UtcDateTime) == date && a.Result == ExecutionResult.Blocked)))
            .ToList();
        var testWork = cycleData.SelectMany(c => c.Items).Where(i => !string.IsNullOrWhiteSpace(i.Assignee)).GroupBy(i => i.Assignee).ToDictionary(g => g.Key, g => g.Count());
        var bugWork = openBugs.Where(b => !string.IsNullOrWhiteSpace(b.Assignee)).GroupBy(b => b.Assignee).ToDictionary(g => g.Key, g => g.Count());
        var workload = testWork.Keys.Union(bugWork.Keys).Select(name => new WorkloadItem(name, testWork.GetValueOrDefault(name), bugWork.GetValueOrDefault(name))).OrderByDescending(x => x.TestItems + x.OpenBugs).Take(10).ToList();

        var bugAges = openBugs.Select(b => Math.Max(0, (int)(now - b.CreatedAtUtc).TotalDays)).ToList();
        var breached = openBugs.Count(b => (now - b.CreatedAtUtc).TotalDays > DashboardCalculator.BugSlaDays(b.Severity.ToString()));
        var agingBands = bugAges.GroupBy(DashboardCalculator.AgingBand).Select(g => new NamedCount(g.Key, g.Count())).OrderBy(x => x.Name).ToList();
        var bugAging = new BugAgingSummary(openBugs.Count - breached, breached, bugAges.Count == 0 ? 0 : Math.Round(bugAges.Average(), 1), agingBands);

        var releaseData = await releases.OrderBy(x => x.TargetDate).ToListAsync(ct);
        var releaseReadiness = releaseData.Select(release =>
        {
            var completed = release.Checklist.Count(i => i.IsRequired && i.IsCompleted);
            var required = release.Checklist.Count(i => i.IsRequired);
            return new ReleaseReadinessItem(release.Id, release.Name, release.Status.ToString(), release.TargetDate, completed, required, DashboardCalculator.Percentage(completed, required));
        }).ToList();

        return new DashboardResponse(
            await products.CountAsync(ct), requirementCount, await testCases.CountAsync(ct), openBugs.Count,
            openBugs.Count(b => b.Severity is BugSeverity.Critical or BugSeverity.High),
            DashboardCalculator.Percentage(coveredRequirements, requirementCount),
            new ExecutionSummary(latest.Count, executed, latest.Count(x => x?.Result == ExecutionResult.Passed), latest.Count(x => x?.Result == ExecutionResult.Failed), latest.Count(x => x?.Result == ExecutionResult.Blocked), latest.Count(x => x?.Result == ExecutionResult.Skipped), DashboardCalculator.Percentage(latest.Count(x => x?.Result == ExecutionResult.Passed), executed)),
            await requirements.GroupBy(x => x.Status).Select(g => new NamedCount(g.Key.ToString(), g.Count())).ToListAsync(ct),
            openBugs.GroupBy(x => x.Severity).Select(g => new NamedCount(g.Key.ToString(), g.Count())).ToList(),
            trend, workload, bugAging, releaseData.Count,
            releaseReadiness.Count == 0 ? 0 : Math.Round(releaseReadiness.Average(x => x.Score), 1), releaseReadiness);
    }
}
