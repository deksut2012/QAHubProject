using QAHub.Api.Domain.TestDesign;namespace QAHub.Api.Tests;
public sealed class TestCaseDomainTests
{
 [Fact]public void TestCaseNormalizesStableCode(){var x=new TestCase(Guid.NewGuid(),null,null," tc-001 ");Assert.Equal("TC-001",x.Code);Assert.Equal(1,x.CurrentVersionNumber);}
 [Fact]public void ReviewWorkflowApprovesDraft(){var x=Version();x.TransitionTo(TestCaseStatus.InReview);x.TransitionTo(TestCaseStatus.Approved);Assert.Equal(TestCaseStatus.Approved,x.Status);}
 [Fact]public void ApprovedVersionIsImmutable(){var x=Version();x.TransitionTo(TestCaseStatus.InReview);x.TransitionTo(TestCaseStatus.Approved);Assert.Throws<InvalidOperationException>(()=>x.Update("Changed","Scenario","",""));}
 [Fact]public void InvalidTransitionIsRejected(){Assert.Throws<InvalidOperationException>(()=>Version().TransitionTo(TestCaseStatus.Active));}
 [Fact]public void CloneWithVersionCopiesContentIntoNewTestCase(){var source=new TestCase(Guid.NewGuid(),null,null,"TC-001");var version=Version();version.Steps.Add(new TestCaseStep(version.Id,1,"Open app","Input","Result"));var clone=source.CloneWithVersion(version,"TC-002");Assert.Equal("TC-002",clone.Code);Assert.Single(clone.Versions);Assert.Equal("Original title",clone.Versions[0].Title);Assert.Single(clone.Versions[0].Steps);Assert.Equal("Open app",clone.Versions[0].Steps[0].Action);}
 private static TestCaseVersion Version()=>new(Guid.NewGuid(),1,"Original title","Scenario","","smoke");
}
