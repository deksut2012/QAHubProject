namespace QAHub.Api.Domain.Integrations;
public sealed class IntegrationConnection
{
 private IntegrationConnection(){}
 public IntegrationConnection(Guid? productId,string name,string provider,string endpoint,string secretReference,string owner){ArgumentException.ThrowIfNullOrWhiteSpace(name);ArgumentException.ThrowIfNullOrWhiteSpace(provider);ArgumentException.ThrowIfNullOrWhiteSpace(owner);if(!string.IsNullOrWhiteSpace(secretReference)&&!secretReference.StartsWith("env:",StringComparison.OrdinalIgnoreCase)&&!secretReference.StartsWith("vault:",StringComparison.OrdinalIgnoreCase))throw new ArgumentException("Secret reference must use env: or vault: and must not contain a secret value.");Id=Guid.NewGuid();ProductId=productId;Name=name.Trim();Provider=provider.Trim();Endpoint=endpoint.Trim();SecretReference=secretReference.Trim();Owner=owner.Trim();IsEnabled=true;CreatedAtUtc=DateTimeOffset.UtcNow;}
 public Guid Id{get;private set;}public Guid? ProductId{get;private set;}public string Name{get;private set;}=string.Empty;public string Provider{get;private set;}=string.Empty;public string Endpoint{get;private set;}=string.Empty;public string SecretReference{get;private set;}=string.Empty;public string Owner{get;private set;}=string.Empty;public bool IsEnabled{get;private set;}public DateTimeOffset CreatedAtUtc{get;private set;}
}
