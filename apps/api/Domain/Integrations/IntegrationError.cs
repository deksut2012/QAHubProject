namespace QAHub.Api.Domain.Integrations;
public sealed class IntegrationError
{
 private IntegrationError(){}
 public IntegrationError(Guid? connectionId,string operation,string externalReference,string code,string message,string owner){ArgumentException.ThrowIfNullOrWhiteSpace(operation);ArgumentException.ThrowIfNullOrWhiteSpace(code);ArgumentException.ThrowIfNullOrWhiteSpace(owner);Id=Guid.NewGuid();ConnectionId=connectionId;Operation=operation.Trim();ExternalReference=externalReference.Trim();Code=code.Trim();Message=message.Trim()[..Math.Min(message.Trim().Length,1000)];Owner=owner.Trim();CreatedAtUtc=DateTimeOffset.UtcNow;NextRetryAtUtc=CreatedAtUtc;}
 public Guid Id{get;private set;}public Guid? ConnectionId{get;private set;}public string Operation{get;private set;}=string.Empty;public string ExternalReference{get;private set;}=string.Empty;public string Code{get;private set;}=string.Empty;public string Message{get;private set;}=string.Empty;public string Owner{get;private set;}=string.Empty;public int RetryCount{get;private set;}public DateTimeOffset NextRetryAtUtc{get;private set;}public bool IsResolved{get;private set;}public DateTimeOffset CreatedAtUtc{get;private set;}
 public void QueueRetry(DateTimeOffset now){if(IsResolved)throw new InvalidOperationException("Resolved integration errors cannot be retried.");RetryCount++;NextRetryAtUtc=now.AddMinutes(Math.Min(60,Math.Pow(2,RetryCount-1)));}
}
