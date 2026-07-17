using QAHub.Api.Domain.Products;

namespace QAHub.Api.Tests;

public sealed class ProductEnvironmentDomainTests
{
    [Fact]
    public void ConstructorNormalizesValues()
    {
        var productId = Guid.NewGuid();
        var environment = new ProductEnvironment(productId, " uat ", " User Acceptance ");
        Assert.Equal(productId, environment.ProductId);
        Assert.Equal("UAT", environment.Code);
        Assert.Equal("User Acceptance", environment.Name);
        Assert.True(environment.IsActive);
    }

    [Fact]
    public void ConstructorRejectsEmptyProductId() =>
        Assert.Throws<ArgumentException>(() => new ProductEnvironment(Guid.Empty, "DEV", "Development"));

    [Fact]
    public void UpdateDoesNotChangeCode()
    {
        var environment = new ProductEnvironment(Guid.NewGuid(), "DEV", "Development");
        environment.Update("Development Local", false);
        Assert.Equal("DEV", environment.Code);
        Assert.Equal("Development Local", environment.Name);
        Assert.False(environment.IsActive);
    }
}
