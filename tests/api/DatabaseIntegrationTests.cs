using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Products;
using QAHub.Api.Domain.Requirements;
using QAHub.Api.Domain.TestDesign;
using QAHub.Api.Domain.Execution;
using QAHub.Api.Domain.Defects;
using QAHub.Api.Domain.Releases;
using QAHub.Api.Domain.Reporting;
using QAHub.Api.Infrastructure.Data;

namespace QAHub.Api.Tests;

public sealed class DatabaseIntegrationTests
{
    [Fact]
    public async Task SqlServerMigrationPersistsProductAndAuditEvent()
    {
        var connection = Environment.GetEnvironmentVariable("QAHUB_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connection)) return;

        var options = new DbContextOptionsBuilder<QAHubDbContext>().UseSqlServer(connection).Options;
        await using var db = new QAHubDbContext(options);
        await db.Database.MigrateAsync();
        var code = $"IT{Guid.NewGuid():N}"[..12].ToUpperInvariant();
        var product = new Product(code, "Integration Test Product");
        db.Products.Add(product);
        await db.SaveChangesAsync();

        Assert.True(await db.Products.AnyAsync(x => x.Id == product.Id));
        Assert.True(await db.AuditEvents.AnyAsync(x => x.EntityId == product.Id && x.Action == "created"));

        var requirement = new Requirement(product.Id, null, $"JOB-{Guid.NewGuid():N}"[..16], "Integration requirement", "Description", "Acceptance criteria", "qa-user");
        db.Requirements.Add(requirement);
        await db.SaveChangesAsync();
        var comment = new RequirementComment(requirement.Id, "qa-user", "Integration comment");
        db.RequirementComments.Add(comment);
        await db.SaveChangesAsync();

        Assert.True(await db.Requirements.AnyAsync(x => x.Id == requirement.Id));
        Assert.True(await db.RequirementComments.AnyAsync(x => x.RequirementId == requirement.Id));
        Assert.True(await db.AuditEvents.AnyAsync(x => x.EntityId == requirement.Id && x.EntityType == nameof(Requirement)));

        var testCase = new TestCase(product.Id, null, requirement.Id, $"TC-{Guid.NewGuid():N}"[..15]);
        var version = new TestCaseVersion(testCase.Id, 1, "Integration test case", "Verify integration", "", "integration");
        version.Steps.Add(new TestCaseStep(version.Id, 1, "Perform action", "Test data", "Expected result"));
        db.TestCases.Add(testCase);
        db.TestCaseVersions.Add(version);
        await db.SaveChangesAsync();

        Assert.True(await db.TestCases.AnyAsync(x => x.Id == testCase.Id));
        Assert.True(await db.TestCaseSteps.AnyAsync(x => x.TestCaseVersionId == version.Id));

        var environment = new ProductEnvironment(product.Id, $"E{Guid.NewGuid():N}"[..10], "Integration");
        var build = new ProductBuild(product.Id, $"IT-{Guid.NewGuid():N}"[..15]);
        db.ProductEnvironments.Add(environment); db.ProductBuilds.Add(build);
        var cycle = new TestCycle(product.Id, environment.Id, build.Id, "Integration cycle", "qa-user");
        var item = new TestCycleItem(cycle.Id, version.Id, "qa-user"); cycle.Items.Add(item); cycle.Start();
        var failed = new TestRunAttempt(item.Id, 1, ExecutionResult.Failed, "500", "log", "unexpected", "qa-user");
        var passed = new TestRunAttempt(item.Id, 2, ExecutionResult.Passed, "fixed", "", "", "qa-user");
        item.Attempts.Add(failed); item.Attempts.Add(passed); db.TestCycles.Add(cycle);
        var bug = new Bug(product.Id, $"BUG-{Guid.NewGuid():N}"[..16], "Integration defect", "", "Reproduce", "Works", "Failed", BugSeverity.High, BugPriority.High, "qa-user");
        bug.RunLinks.Add(new BugRunLink(bug.Id, failed.Id)); bug.Assign("developer");
        bug.TransitionTo(BugStatus.Triaged, "lead", "triaged"); bug.TransitionTo(BugStatus.Assigned, "lead", "assigned"); bug.TransitionTo(BugStatus.InProgress, "developer", "working"); bug.TransitionTo(BugStatus.Fixed, "developer", "fixed", build.Id); bug.TransitionTo(BugStatus.ReadyForRetest, "developer", "ready"); bug.TransitionTo(BugStatus.Verified, "qa-user", "passed", verificationAttemptId: passed.Id); bug.TransitionTo(BugStatus.Closed, "lead", "verified");
        bug.Comments.Add(new BugComment(bug.Id, "qa-user", "Integration comment")); bug.EvidenceFiles.Add(new BugEvidence(bug.Id, "result.txt", "text/plain", "passed"u8.ToArray(), "qa-user"));
        db.Bugs.Add(bug); await db.SaveChangesAsync();
        Assert.True(await db.Bugs.AnyAsync(x=>x.Id==bug.Id&&x.Status==BugStatus.Closed&&x.FixBuildId==build.Id&&x.VerificationAttemptId==passed.Id));
        Assert.True(await db.BugRunLinks.AnyAsync(x=>x.BugId==bug.Id&&x.TestRunAttemptId==failed.Id));
        Assert.Equal(7, await db.BugStatusHistories.CountAsync(x=>x.BugId==bug.Id));

        requirement.TransitionTo(RequirementStatus.InReview); requirement.TransitionTo(RequirementStatus.Approved); requirement.TransitionTo(RequirementStatus.Implemented); requirement.TransitionTo(RequirementStatus.Verified); cycle.Complete();
        var release = new ReleaseCandidate(product.Id, build.Id, environment.Id, cycle.Id, "Integration release", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), "Release notes", "Rollback plan");
        release.Requirements.Add(new ReleaseRequirement(release.Id, requirement.Id)); foreach(var check in release.Checklist)check.SetCompleted(true,"qa-user"); release.MarkCandidate(); release.SignOff(SignOffDecision.Approved,"qa-lead","",true); release.RecordDeployment(DeploymentStatus.Deployed,"deployed",true,"qa-user");
        db.Releases.Add(release); await db.SaveChangesAsync();
        Assert.True(await db.Releases.AnyAsync(x=>x.Id==release.Id&&x.Status==ReleaseStatus.Released&&x.PostReleaseValidated));
        Assert.True(await db.ReleaseRequirements.AnyAsync(x=>x.ReleaseId==release.Id&&x.RequirementId==requirement.Id));

        var scheduledReport = new ScheduledReport(product.Id, "Integration dashboard report", ReportFrequency.Weekly, new TimeOnly(8, 0), "qa@example.com", DateTimeOffset.UtcNow);
        db.ScheduledReports.Add(scheduledReport); await db.SaveChangesAsync();
        Assert.True(await db.ScheduledReports.AnyAsync(x => x.Id == scheduledReport.Id && x.IsEnabled));

        var latestResults = await db.TestCycleItems.Where(x => x.TestCycleId == cycle.Id)
            .Select(x => x.Attempts.OrderByDescending(a => a.AttemptNumber).Select(a => (ExecutionResult?)a.Result).FirstOrDefault()).ToListAsync();
        var reconciled = QAHub.Api.Features.Reporting.DashboardCalculator.ReconcileExecution(latestResults);
        Assert.Equal(await db.TestCycleItems.CountAsync(x => x.TestCycleId == cycle.Id), reconciled.Total);
        Assert.Equal(1, reconciled.Passed);
    }
}
