namespace QAHub.Api.Domain.Requirements;

public sealed class Requirement
{
    private static readonly Dictionary<RequirementStatus, RequirementStatus[]> Transitions =
        new Dictionary<RequirementStatus, RequirementStatus[]>
        {
            [RequirementStatus.Draft] = [RequirementStatus.InReview],
            [RequirementStatus.InReview] = [RequirementStatus.Approved, RequirementStatus.NeedsRevision],
            [RequirementStatus.NeedsRevision] = [RequirementStatus.InReview],
            [RequirementStatus.Approved] = [RequirementStatus.Implemented, RequirementStatus.Superseded],
            [RequirementStatus.Implemented] = [RequirementStatus.Verified],
            [RequirementStatus.Verified] = [RequirementStatus.Closed],
        };

    private Requirement() { }
    public Requirement(Guid productId, Guid? moduleId, string jobNumber, string title, string description, string acceptanceCriteria, string? assignee)
    {
        Id = Guid.NewGuid(); ProductId = productId; ModuleId = moduleId; JobNumber = jobNumber.Trim().ToUpperInvariant();
        Title = title.Trim(); Description = description.Trim(); AcceptanceCriteria = acceptanceCriteria.Trim(); Assignee = Normalize(assignee);
        Status = RequirementStatus.Draft; CreatedAtUtc = UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid? ModuleId { get; private set; }
    public string JobNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string AcceptanceCriteria { get; private set; } = string.Empty;
    public string? Assignee { get; private set; }
    public RequirementStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public void Update(Guid? moduleId, string title, string description, string acceptanceCriteria, string? assignee)
    { if (Status is not RequirementStatus.Draft and not RequirementStatus.NeedsRevision) throw new InvalidOperationException("Only draft or revision requirements can be edited."); ModuleId=moduleId;Title=title.Trim();Description=description.Trim();AcceptanceCriteria=acceptanceCriteria.Trim();Assignee=Normalize(assignee);UpdatedAtUtc=DateTimeOffset.UtcNow; }
    public void TransitionTo(RequirementStatus next)
    { if (!Transitions.TryGetValue(Status, out var allowed) || !allowed.Contains(next)) throw new InvalidOperationException($"Transition from {Status} to {next} is not allowed."); Status=next;UpdatedAtUtc=DateTimeOffset.UtcNow; }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
