using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Products;
using QAHub.Api.Infrastructure.Data;
using QAHub.Api.Infrastructure.Security;

namespace QAHub.Api.Features.Products;

public sealed record CreateProductEnvironmentRequest(string? Code, string? Name);
public sealed record ProductEnvironmentResponse(Guid Id, Guid ProductId, string Code, string Name, bool IsActive);

public static class ProductEnvironmentEndpoints
{
    public static IEndpointRouteBuilder MapProductEnvironmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/products/{productId:guid}/environments")
            .WithTags("Product Environments")
            .RequireAuthorization(AuthorizationPolicies.ProductAccess);
        group.MapGet("/", GetAll).WithName("GetProductEnvironments");
        group.MapPost("/", Create).WithName("CreateProductEnvironment");
        return endpoints;
    }

    private static async Task<IResult> GetAll(Guid productId, QAHubDbContext db, CancellationToken cancellationToken)
    {
        if (!await db.Products.AnyAsync(x => x.Id == productId, cancellationToken)) return TypedResults.NotFound();
        var items = await db.ProductEnvironments.AsNoTracking().Where(x => x.ProductId == productId)
            .OrderBy(x => x.Code)
            .Select(x => new ProductEnvironmentResponse(x.Id, x.ProductId, x.Code, x.Name, x.IsActive))
            .ToListAsync(cancellationToken);
        return TypedResults.Ok(new { items, totalCount = items.Count });
    }

    private static async Task<IResult> Create(Guid productId, CreateProductEnvironmentRequest request, QAHubDbContext db, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(request.Code)) errors["code"] = ["Environment code is required."];
        else if (request.Code.Trim().Length > 32) errors["code"] = ["Environment code must not exceed 32 characters."];
        if (string.IsNullOrWhiteSpace(request.Name)) errors["name"] = ["Environment name is required."];
        else if (request.Name.Trim().Length > 200) errors["name"] = ["Environment name must not exceed 200 characters."];
        if (errors.Count > 0) return TypedResults.ValidationProblem(errors);

        var product = await db.Products.AsNoTracking().SingleOrDefaultAsync(x => x.Id == productId, cancellationToken);
        if (product is null || !product.IsActive) return TypedResults.NotFound();
        var code = request.Code!.Trim().ToUpperInvariant();
        if (await db.ProductEnvironments.AnyAsync(x => x.ProductId == productId && x.Code == code, cancellationToken)) return TypedResults.Conflict();
        var environment = new ProductEnvironment(productId, code, request.Name!);
        db.ProductEnvironments.Add(environment);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Created($"/api/v1/products/{productId}/environments/{environment.Id}",
            new ProductEnvironmentResponse(environment.Id, environment.ProductId, environment.Code, environment.Name, environment.IsActive));
    }
}
