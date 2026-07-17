using Microsoft.EntityFrameworkCore;
using QAHub.Api.Features.Products;
using QAHub.Api.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var databaseConnection = builder.Configuration.GetConnectionString("QAHub")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=QAHub;Integrated Security=True";
builder.Services.AddDbContext<QAHubDbContext>(options =>
    options.UseSqlServer(databaseConnection, sql => sql.EnableRetryOnFailure()));

var app = builder.Build();

app.UseExceptionHandler();

app.Use(async (context, next) =>
{
    const string correlationHeader = "X-Correlation-ID";
    var correlationId = context.Request.Headers[correlationHeader].FirstOrDefault()
        ?? context.TraceIdentifier;

    context.TraceIdentifier = correlationId;
    context.Response.Headers[correlationHeader] = correlationId;
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");

app.MapGet("/api/v1/system/info", () => Results.Ok(new
    {
        service = "QAHub.Api",
        version = "v1",
        status = "ready"
    }))
    .WithName("GetSystemInfo");

app.MapProductEndpoints();
app.MapProductModuleEndpoints();

app.Run();
