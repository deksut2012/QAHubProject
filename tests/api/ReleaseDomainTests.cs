using QAHub.Api.Domain.Releases;
namespace QAHub.Api.Tests;
public sealed class ReleaseDomainTests
{
 private static ReleaseCandidate NewRelease()=>new(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),null,"Release 1",DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),"Notes","Rollback");
 [Fact]public void ReleaseStartsAsDraftWithRequiredChecklist(){var x=NewRelease();Assert.Equal(ReleaseStatus.Draft,x.Status);Assert.Equal(3,x.Checklist.Count);Assert.All(x.Checklist,i=>Assert.True(i.IsRequired));}
 [Fact]public void ApprovedSignOffIsBlockedWhenCriticalGateFails(){var x=NewRelease();x.MarkCandidate();Assert.Throws<InvalidOperationException>(()=>x.SignOff(SignOffDecision.Approved,"lead","",false));}
 [Fact]public void ConditionalSignOffRequiresReason(){var x=NewRelease();x.MarkCandidate();Assert.Throws<InvalidOperationException>(()=>x.SignOff(SignOffDecision.Conditional,"lead","",false));}
 [Fact]public void ApprovedSignOffSucceedsWhenCriticalGatesPass(){var x=NewRelease();x.MarkCandidate();x.SignOff(SignOffDecision.Approved,"lead","",true);Assert.Equal(ReleaseStatus.Approved,x.Status);Assert.NotNull(x.SignedOffAtUtc);}
}
