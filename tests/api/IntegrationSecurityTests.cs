using QAHub.Api.Domain.Integrations;using QAHub.Api.Features.Integrations;
namespace QAHub.Api.Tests;
public sealed class IntegrationSecurityTests
{
 [Fact]public void SecretComparisonAcceptsExactValueOnly(){Assert.True(SecretVerifier.Matches("strong-secret","strong-secret"));Assert.False(SecretVerifier.Matches("strong-secret","wrong"));Assert.False(SecretVerifier.Matches("strong-secret",null));}
 [Fact]public void AutomationRunRejectsInconsistentCounts()=>Assert.Throws<ArgumentOutOfRangeException>(()=>new AutomationRun(Guid.NewGuid(),"GitHub","1","main","abc","failed",2,2,1,0,"DevOps","HASH"));
}
