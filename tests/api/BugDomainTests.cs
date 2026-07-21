using QAHub.Api.Domain.Defects;
namespace QAHub.Api.Tests;
public sealed class BugDomainTests
{
 private static Bug NewBug()=>new(Guid.NewGuid(),"BUG-1","Broken checkout","Details","1. Open cart","Order completes","500 error",BugSeverity.Critical,BugPriority.Urgent,"qa");
 [Fact]public void NewBugStartsWithAgingMetadata(){var bug=NewBug();Assert.Equal(BugStatus.New,bug.Status);Assert.Equal(BugSeverity.Critical,bug.Severity);}
 [Fact]public void AssignedRequiresAssignee(){var bug=NewBug();bug.TransitionTo(BugStatus.Triaged,"lead","");Assert.Throws<InvalidOperationException>(()=>bug.TransitionTo(BugStatus.Assigned,"lead",""));}
 [Fact]public void FixedRequiresFixBuild(){var bug=NewBug();bug.TransitionTo(BugStatus.Triaged,"lead","");bug.Assign("dev");bug.TransitionTo(BugStatus.Assigned,"lead","");bug.TransitionTo(BugStatus.InProgress,"dev","");Assert.Throws<InvalidOperationException>(()=>bug.TransitionTo(BugStatus.Fixed,"dev",""));}
 [Fact]public void DuplicateRequiresCanonicalBug(){var bug=NewBug();Assert.Throws<InvalidOperationException>(()=>bug.TransitionTo(BugStatus.Duplicate,"lead",""));}
 [Fact]public void ReopenRequiresReason(){var bug=NewBug();bug.TransitionTo(BugStatus.Triaged,"lead","");bug.Assign("dev");bug.TransitionTo(BugStatus.Assigned,"lead","");bug.TransitionTo(BugStatus.InProgress,"dev","");bug.TransitionTo(BugStatus.Fixed,"dev","",Guid.NewGuid());bug.TransitionTo(BugStatus.ReadyForRetest,"dev","");Assert.Throws<InvalidOperationException>(()=>bug.TransitionTo(BugStatus.Reopened,"qa",""));}
 [Fact]public void VerifiedRequiresPassedRetestReference(){var bug=NewBug();bug.TransitionTo(BugStatus.Triaged,"lead","");bug.Assign("dev");bug.TransitionTo(BugStatus.Assigned,"lead","");bug.TransitionTo(BugStatus.InProgress,"dev","");bug.TransitionTo(BugStatus.Fixed,"dev","",Guid.NewGuid());bug.TransitionTo(BugStatus.ReadyForRetest,"dev","");Assert.Throws<InvalidOperationException>(()=>bug.TransitionTo(BugStatus.Verified,"qa",""));}
 [Fact]public void CommentRequiresBody()=>Assert.Throws<ArgumentException>(()=>new BugComment(Guid.NewGuid(),"qa",""));
 [Fact]public void EvidenceRejectsFilesOverTenMegabytes()=>Assert.Throws<ArgumentException>(()=>new BugEvidence(Guid.NewGuid(),"large.bin","application/octet-stream",new byte[10*1024*1024+1],"qa"));
 [Fact]public void RelatedBugCannotReferenceItself(){var id=Guid.NewGuid();Assert.Throws<ArgumentException>(()=>new BugRelation(id,id,"qa"));}
}
