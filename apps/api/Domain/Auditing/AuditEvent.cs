namespace QAHub.Api.Domain.Auditing;

public sealed class AuditEvent
{
    private AuditEvent() { }

    public AuditEvent(
        string actorId,
        string action,
        string entityType,
        Guid entityId,
        Guid? productId,
        string correlationId,
        string changesJson)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityType);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        Id = Guid.NewGuid();
        OccurredAtUtc = DateTimeOffset.UtcNow;
        ActorId = actorId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        ProductId = productId;
        CorrelationId = correlationId;
        ChangesJson = changesJson;
    }

    public Guid Id { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }
    public string ActorId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public Guid? ProductId { get; private set; }
    public string CorrelationId { get; private set; } = string.Empty;
    public string ChangesJson { get; private set; } = "{}";
}
