namespace QAHub.Api.Domain.Products;

public sealed class ProductModule
{
    private ProductModule() { }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public Product Product { get; private set; } = null!;
}
