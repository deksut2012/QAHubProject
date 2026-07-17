namespace QAHub.Api.Features.Products;

public sealed record CreateProductModuleRequest(string? Code, string? Name);

public sealed record UpdateProductModuleRequest(string? Name, bool IsActive);

public sealed record ProductModuleResponse(
    Guid Id,
    Guid ProductId,
    string Code,
    string Name,
    bool IsActive);

public sealed record ProductModuleCollectionResponse(
    IReadOnlyList<ProductModuleResponse> Items,
    int TotalCount);
