namespace QAHub.Api.Domain.Execution;
public sealed class TestRunEvidence
{
 private TestRunEvidence(){}
 public TestRunEvidence(Guid attemptId,string fileName,string contentType,byte[] content,string uploadedBy){if(content.Length is 0 or >10_485_760)throw new ArgumentException("Evidence must be between 1 byte and 10 MB.");Id=Guid.NewGuid();TestRunAttemptId=attemptId;FileName=Path.GetFileName(fileName);ContentType=string.IsNullOrWhiteSpace(contentType)?"application/octet-stream":contentType;Content=content;SizeBytes=content.LongLength;UploadedBy=uploadedBy;UploadedAtUtc=DateTimeOffset.UtcNow;}
 public Guid Id{get;private set;}public Guid TestRunAttemptId{get;private set;}public string FileName{get;private set;}=string.Empty;public string ContentType{get;private set;}=string.Empty;public long SizeBytes{get;private set;}public byte[] Content{get;private set;}=[];public string UploadedBy{get;private set;}=string.Empty;public DateTimeOffset UploadedAtUtc{get;private set;}
}
