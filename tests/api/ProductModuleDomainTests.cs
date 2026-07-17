using QAHub.Api.Domain.Products;

namespace QAHub.Api.Tests;

public sealed class ProductModuleDomainTests
{
    [Fact]
    public void ConstructorNormalizesValues()
    {
        var productId = Guid.NewGuid();

        var module = new ProductModule(productId, " stk ", " Stock ");

        Assert.Equal(productId, module.ProductId);
        Assert.Equal("STK", module.Code);
        Assert.Equal("Stock", module.Name);
        Assert.True(module.IsActive);
    }

    [Fact]
    public void ConstructorRejectsEmptyProductId()
    {
        Assert.Throws<ArgumentException>(() => new ProductModule(Guid.Empty, "STK", "Stock"));
    }

    [Theory]
    [InlineData(null, "Stock")]
    [InlineData("", "Stock")]
    [InlineData("STK", null)]
    [InlineData("STK", " ")]
    public void ConstructorRejectsBlankRequiredValues(string? code, string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new ProductModule(Guid.NewGuid(), code!, name!));
    }

    [Fact]
    public void UpdateKeepsIdentityAndCode()
    {
        var module = new ProductModule(Guid.NewGuid(), "STK", "Stock");
        var id = module.Id;

        module.Update("Inventory", false);

        Assert.Equal(id, module.Id);
        Assert.Equal("STK", module.Code);
        Assert.Equal("Inventory", module.Name);
        Assert.False(module.IsActive);
    }
}
