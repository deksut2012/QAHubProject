using QAHub.Api.Domain.Products;

namespace QAHub.Api.Tests;

public sealed class ProductDomainTests
{
    [Fact]
    public void ConstructorNormalizesCodeAndName()
    {
        var product = new Product(" pmx ", " ProMaxx ");

        Assert.Equal("PMX", product.Code);
        Assert.Equal("ProMaxx", product.Name);
        Assert.True(product.IsActive);
    }

    [Theory]
    [InlineData(null, "Product")]
    [InlineData("", "Product")]
    [InlineData("PMX", null)]
    [InlineData("PMX", " ")]
    public void ConstructorRejectsBlankRequiredValues(string? code, string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Product(code!, name!));
    }

    [Fact]
    public void UpdateChangesMutableFieldsButKeepsCode()
    {
        var product = new Product("PMX", "ProMaxx");

        product.Update("ProMaxx ERP", false);

        Assert.Equal("PMX", product.Code);
        Assert.Equal("ProMaxx ERP", product.Name);
        Assert.False(product.IsActive);
        Assert.NotNull(product.UpdatedAtUtc);
    }
}
