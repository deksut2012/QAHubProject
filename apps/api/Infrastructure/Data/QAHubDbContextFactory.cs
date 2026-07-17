using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QAHub.Api.Infrastructure.Data;

public sealed class QAHubDbContextFactory : IDesignTimeDbContextFactory<QAHubDbContext>
{
    public QAHubDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__QAHub")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=QAHub;Integrated Security=True";
        var options = new DbContextOptionsBuilder<QAHubDbContext>()
            .UseSqlServer(connection)
            .Options;
        return new QAHubDbContext(options);
    }
}
