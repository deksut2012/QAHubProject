namespace QAHub.Api.Features.Products;

public sealed record CreateProductRequest(string? Code, string? Name);

public sealed record UpdateProductRequest(string? Name, bool IsActive);

public sealed record ProductResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record ProductCollectionResponse(
    IReadOnlyList<ProductResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);
