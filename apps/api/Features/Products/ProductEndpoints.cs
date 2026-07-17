using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Products;
using QAHub.Api.Infrastructure.Data;

namespace QAHub.Api.Features.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/products").WithTags("Products");
        group.MapGet("/", GetProducts).WithName("GetProducts");
        group.MapGet("/{id:guid}", GetProduct).WithName("GetProduct");
        group.MapPost("/", CreateProduct).WithName("CreateProduct");
        group.MapPut("/{id:guid}", UpdateProduct).WithName("UpdateProduct");
        return endpoints;
    }

    private static async Task<Ok<ProductCollectionResponse>> GetProducts(
        QAHubDbContext db,
        string? search,
        bool? isActive,
        int page = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Code.Contains(term) || x.Name.Contains(term));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProductResponse(
                x.Id,
                x.Code,
                x.Name,
                x.IsActive,
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(new ProductCollectionResponse(items, page, pageSize, totalCount));
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound>> GetProduct(
        Guid id,
        QAHubDbContext db,
        CancellationToken cancellationToken)
    {
        var product = await db.Products.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return product is null ? TypedResults.NotFound() : TypedResults.Ok(ToResponse(product));
    }

    private static async Task<Results<Created<ProductResponse>, ValidationProblem, Conflict>> CreateProduct(
        CreateProductRequest request,
        QAHubDbContext db,
        CancellationToken cancellationToken)
    {
        var errors = Validate(request.Code, request.Name);
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var code = request.Code!.Trim().ToUpperInvariant();
        if (await db.Products.AnyAsync(x => x.Code == code, cancellationToken))
        {
            return TypedResults.Conflict();
        }

        var product = new Product(code, request.Name!);
        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Created($"/api/v1/products/{product.Id}", ToResponse(product));
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound, ValidationProblem>> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        QAHubDbContext db,
        CancellationToken cancellationToken)
    {
        var errors = ValidateName(request.Name);
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var product = await db.Products.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null)
        {
            return TypedResults.NotFound();
        }

        product.Update(request.Name!, request.IsActive);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(ToResponse(product));
    }

    private static Dictionary<string, string[]> Validate(string? code, string? name)
    {
        var errors = ValidateName(name);
        if (string.IsNullOrWhiteSpace(code))
        {
            errors["code"] = ["Product code is required."];
        }
        else if (code.Trim().Length > 32)
        {
            errors["code"] = ["Product code must not exceed 32 characters."];
        }

        return errors;
    }

    private static Dictionary<string, string[]> ValidateName(string? name)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(name))
        {
            errors["name"] = ["Product name is required."];
        }
        else if (name.Trim().Length > 200)
        {
            errors["name"] = ["Product name must not exceed 200 characters."];
        }

        return errors;
    }

    private static ProductResponse ToResponse(Product product) =>
        new(product.Id, product.Code, product.Name, product.IsActive, product.CreatedAtUtc, product.UpdatedAtUtc);
}
