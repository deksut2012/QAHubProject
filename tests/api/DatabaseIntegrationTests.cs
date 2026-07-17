using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Products;
using QAHub.Api.Domain.Requirements;
using QAHub.Api.Domain.TestDesign;
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
    }
}
