using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Products;
using QAHub.Api.Infrastructure.Data;

namespace QAHub.Api.Features.Products;

public static class ProductModuleEndpoints
{
    public static IEndpointRouteBuilder MapProductModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/products/{productId:guid}/modules")
            .WithTags("Product Modules");
        group.MapGet("/", GetModules).WithName("GetProductModules");
        group.MapGet("/{id:guid}", GetModule).WithName("GetProductModule");
        group.MapPost("/", CreateModule).WithName("CreateProductModule");
        group.MapPut("/{id:guid}", UpdateModule).WithName("UpdateProductModule");
        return endpoints;
    }

    private static async Task<Results<Ok<ProductModuleCollectionResponse>, NotFound>> GetModules(
        Guid productId,
        QAHubDbContext db,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        if (!await db.Products.AnyAsync(x => x.Id == productId, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var query = db.ProductModules.AsNoTracking().Where(x => x.ProductId == productId);
        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        var items = await query
            .OrderBy(x => x.Code)
            .Select(x => new ProductModuleResponse(x.Id, x.ProductId, x.Code, x.Name, x.IsActive))
            .ToListAsync(cancellationToken);
        return TypedResults.Ok(new ProductModuleCollectionResponse(items, items.Count));
    }

    private static async Task<Results<Ok<ProductModuleResponse>, NotFound>> GetModule(
        Guid productId,
        Guid id,
        QAHubDbContext db,
        CancellationToken cancellationToken)
    {
        var module = await db.ProductModules.AsNoTracking()
            .SingleOrDefaultAsync(x => x.ProductId == productId && x.Id == id, cancellationToken);
        return module is null ? TypedResults.NotFound() : TypedResults.Ok(ToResponse(module));
    }

    private static async Task<Results<Created<ProductModuleResponse>, NotFound, ValidationProblem, Conflict>> CreateModule(
        Guid productId,
        CreateProductModuleRequest request,
        QAHubDbContext db,
        CancellationToken cancellationToken)
    {
        var errors = Validate(request.Code, request.Name);
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var product = await db.Products.AsNoTracking().SingleOrDefaultAsync(x => x.Id == productId, cancellationToken);
        if (product is null || !product.IsActive)
        {
            return TypedResults.NotFound();
        }

        var code = request.Code!.Trim().ToUpperInvariant();
        if (await db.ProductModules.AnyAsync(x => x.ProductId == productId && x.Code == code, cancellationToken))
        {
            return TypedResults.Conflict();
        }

        var module = new ProductModule(productId, code, request.Name!);
        db.ProductModules.Add(module);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Created($"/api/v1/products/{productId}/modules/{module.Id}", ToResponse(module));
    }

    private static async Task<Results<Ok<ProductModuleResponse>, NotFound, ValidationProblem>> UpdateModule(
        Guid productId,
        Guid id,
        UpdateProductModuleRequest request,
        QAHubDbContext db,
        CancellationToken cancellationToken)
    {
        var errors = ValidateName(request.Name);
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var module = await db.ProductModules
            .SingleOrDefaultAsync(x => x.ProductId == productId && x.Id == id, cancellationToken);
        if (module is null)
        {
            return TypedResults.NotFound();
        }

        module.Update(request.Name!, request.IsActive);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(ToResponse(module));
    }

    private static Dictionary<string, string[]> Validate(string? code, string? name)
    {
        var errors = ValidateName(name);
        if (string.IsNullOrWhiteSpace(code))
        {
            errors["code"] = ["Module code is required."];
        }
        else if (code.Trim().Length > 32)
        {
            errors["code"] = ["Module code must not exceed 32 characters."];
        }

        return errors;
    }

    private static Dictionary<string, string[]> ValidateName(string? name)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(name))
        {
            errors["name"] = ["Module name is required."];
        }
        else if (name.Trim().Length > 200)
        {
            errors["name"] = ["Module name must not exceed 200 characters."];
        }

        return errors;
    }

    private static ProductModuleResponse ToResponse(ProductModule module) =>
        new(module.Id, module.ProductId, module.Code, module.Name, module.IsActive);
}
