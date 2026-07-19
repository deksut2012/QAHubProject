namespace QAHub.Api.Domain.TestDesign;

public sealed class TestCaseHistoryEntry
{
    private TestCaseHistoryEntry() { }

    public TestCaseHistoryEntry(Guid versionId, TestCaseStatus status, string actorId, DateTimeOffset occurredAtUtc)
    {
        Id = Guid.NewGuid();
        TestCaseVersionId = versionId;
        Status = status;
        ActorId = actorId;
        OccurredAtUtc = occurredAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid TestCaseVersionId { get; private set; }
    public TestCaseStatus Status { get; private set; }
    public string ActorId { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAtUtc { get; private set; }
}
