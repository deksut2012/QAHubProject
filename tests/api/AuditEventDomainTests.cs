using QAHub.Api.Domain.Auditing;

namespace QAHub.Api.Tests;

public sealed class AuditEventDomainTests
{
    [Fact]
    public void ConstructorCapturesImmutableEventData()
    {
        var entityId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var auditEvent = new AuditEvent("tester", "created", "Product", entityId, productId, "trace-1", "{}");

        Assert.NotEqual(Guid.Empty, auditEvent.Id);
        Assert.Equal("tester", auditEvent.ActorId);
        Assert.Equal("created", auditEvent.Action);
        Assert.Equal(entityId, auditEvent.EntityId);
        Assert.Equal(productId, auditEvent.ProductId);
        Assert.Equal("trace-1", auditEvent.CorrelationId);
    }

    [Fact]
    public void ConstructorRejectsMissingActor() =>
        Assert.ThrowsAny<ArgumentException>(() => new AuditEvent("", "created", "Product", Guid.NewGuid(), null, "trace", "{}"));
}
