namespace QAHub.Api.Domain.Requirements;

public sealed class RequirementComment
{
    private RequirementComment() { }
    public RequirementComment(Guid requirementId, string authorId, string body)
    { Id=Guid.NewGuid();RequirementId=requirementId;AuthorId=authorId;Body=body.Trim();CreatedAtUtc=DateTimeOffset.UtcNow; }
    public Guid Id { get; private set; }
    public Guid RequirementId { get; private set; }
    public string AuthorId { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
}
