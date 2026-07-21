using Microsoft.EntityFrameworkCore;
using QAHub.Api.Infrastructure.Security;
using QAHub.Api.Features.Products;
using QAHub.Api.Features.Requirements;
using QAHub.Api.Features.TestDesign;
using QAHub.Api.Features.Execution;
using QAHub.Api.Features.Administration;
using QAHub.Api.Features.Auditing;
using QAHub.Api.Features.Defects;
using QAHub.Api.Features.Releases;
using QAHub.Api.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["code"] = $"HTTP_{context.ProblemDetails.Status}";
    };
});
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddQAHubSecurity(builder.Configuration, builder.Environment);

var databaseConnection = builder.Configuration.GetConnectionString("QAHub")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=QAHub;Integrated Security=True";
builder.Services.AddDbContext<QAHubDbContext>(options =>
    options.UseSqlServer(databaseConnection, sql => sql.EnableRetryOnFailure()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages(async statusContext =>
{
    var response = statusContext.HttpContext.Response;
    await Results.Problem(
        statusCode: response.StatusCode,
        title: response.StatusCode switch
        {
            StatusCodes.Status401Unauthorized => "Authentication required",
            StatusCodes.Status403Forbidden => "Access denied",
            StatusCodes.Status404NotFound => "Resource not found",
            StatusCodes.Status409Conflict => "Resource conflict",
            _ => "Request failed",
        },
        extensions: new Dictionary<string, object?>
        {
            ["traceId"] = statusContext.HttpContext.TraceIdentifier,
            ["code"] = $"HTTP_{response.StatusCode}",
        }).ExecuteAsync(statusContext.HttpContext);
});

app.Use(async (context, next) =>
{
    const string correlationHeader = "X-Correlation-ID";
    var correlationId = context.Request.Headers[correlationHeader].FirstOrDefault()
        ?? context.TraceIdentifier;

    context.TraceIdentifier = correlationId;
    context.Response.Headers[correlationHeader] = correlationId;
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

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

app.MapGet("/api/v1/session", (System.Security.Claims.ClaimsPrincipal user) => Results.Ok(new
    {
        id = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
        name = user.Identity?.Name,
        roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(claim => claim.Value)
    }))
    .RequireAuthorization()
    .WithName("GetSession");

app.MapProductEndpoints();
app.MapProductModuleEndpoints();
app.MapProductEnvironmentEndpoints();
app.MapRequirementEndpoints();
app.MapTestCaseEndpoints();
app.MapExecutionEndpoints();
app.MapBugEndpoints();
app.MapReleaseEndpoints();
app.MapAdministrationEndpoints();
app.MapAuditEndpoints();

app.Run();
