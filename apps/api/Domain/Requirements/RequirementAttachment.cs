namespace QAHub.Api.Domain.Requirements;

public sealed class RequirementAttachment
{
    private RequirementAttachment() { }
    public RequirementAttachment(Guid requirementId,string fileName,string contentType,byte[] content,string uploadedBy)
    { Id=Guid.NewGuid();RequirementId=requirementId;FileName=fileName.Trim();ContentType=contentType.Trim();Content=content;SizeBytes=content.LongLength;UploadedBy=uploadedBy;UploadedAtUtc=DateTimeOffset.UtcNow; }
    public Guid Id { get; private set; }
    public Guid RequirementId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = "application/octet-stream";
    public long SizeBytes { get; private set; }
    public byte[] Content { get; private set; } = [];
    public string UploadedBy { get; private set; } = string.Empty;
    public DateTimeOffset UploadedAtUtc { get; private set; }
}
