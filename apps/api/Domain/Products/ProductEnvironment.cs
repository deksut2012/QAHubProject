namespace QAHub.Api.Domain.Products;

public sealed class ProductEnvironment
{
    private ProductEnvironment() { }

    public ProductEnvironment(Guid productId, string code, string name)
    {
        if (productId == Guid.Empty) throw new ArgumentException("Product ID is required.", nameof(productId));
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = Guid.NewGuid();
        ProductId = productId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public Product Product { get; private set; } = null!;

    public void Update(string name, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        IsActive = isActive;
    }
}
