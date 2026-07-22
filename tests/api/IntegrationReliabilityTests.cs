using QAHub.Api.Domain.Integrations;
namespace QAHub.Api.Tests;
public sealed class IntegrationReliabilityTests
{
 [Theory][InlineData("plain-secret")][InlineData("https://vault/value")]
 public void ConnectionRejectsPlaintextSecret(string secret)=>Assert.Throws<ArgumentException>(()=>new IntegrationConnection(null,"GitHub","GitHub","https://github.com/org/repo",secret,"DevOps"));
 [Theory][InlineData("env:GITHUB_TOKEN")][InlineData("vault:qahub/github")]
 public void ConnectionAcceptsSecretReference(string secret)=>Assert.Equal(secret,new IntegrationConnection(null,"GitHub","GitHub","https://github.com/org/repo",secret,"DevOps").SecretReference);
 [Fact]public void RetryUsesCappedExponentialBackoff(){var now=DateTimeOffset.UtcNow;var x=new IntegrationError(null,"Sync","run-1","TIMEOUT","Timed out","DevOps");x.QueueRetry(now);Assert.Equal(now.AddMinutes(1),x.NextRetryAtUtc);for(var i=0;i<10;i++)x.QueueRetry(now);Assert.Equal(now.AddMinutes(60),x.NextRetryAtUtc);}
}
