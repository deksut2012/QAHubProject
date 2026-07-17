using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Products;
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
    }
}
