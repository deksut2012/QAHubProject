using QAHub.Api.Domain.TestDesign;namespace QAHub.Api.Tests;
public sealed class TestCaseDomainTests
{
 [Fact]public void TestCaseNormalizesStableCode(){var x=new TestCase(Guid.NewGuid(),null,null," tc-001 ");Assert.Equal("TC-001",x.Code);Assert.Equal(1,x.CurrentVersionNumber);}
 [Fact]public void ReviewWorkflowApprovesDraft(){var x=Version();x.TransitionTo(TestCaseStatus.InReview);x.TransitionTo(TestCaseStatus.Approved);Assert.Equal(TestCaseStatus.Approved,x.Status);}
 [Fact]public void ApprovedVersionIsImmutable(){var x=Version();x.TransitionTo(TestCaseStatus.InReview);x.TransitionTo(TestCaseStatus.Approved);Assert.Throws<InvalidOperationException>(()=>x.Update("Changed","Scenario","",""));}
 [Fact]public void InvalidTransitionIsRejected(){Assert.Throws<InvalidOperationException>(()=>Version().TransitionTo(TestCaseStatus.Active));}
 private static TestCaseVersion Version()=>new(Guid.NewGuid(),1,"Title","Scenario","","smoke");
}
