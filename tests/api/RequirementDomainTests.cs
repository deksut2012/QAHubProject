using QAHub.Api.Domain.Requirements;
namespace QAHub.Api.Tests;
public sealed class RequirementDomainTests
{
 [Fact] public void NewRequirementStartsAsNormalizedDraft(){var x=Create(" job-1 ");Assert.Equal("JOB-1",x.JobNumber);Assert.Equal(RequirementStatus.Draft,x.Status);}
 [Fact] public void ValidWorkflowCanReachApproved(){var x=Create();x.TransitionTo(RequirementStatus.InReview);x.TransitionTo(RequirementStatus.Approved);Assert.Equal(RequirementStatus.Approved,x.Status);}
 [Fact] public void InvalidWorkflowIsRejected(){var x=Create();Assert.Throws<InvalidOperationException>(()=>x.TransitionTo(RequirementStatus.Approved));}
 [Fact] public void ApprovedRequirementCannotBeEdited(){var x=Create();x.TransitionTo(RequirementStatus.InReview);x.TransitionTo(RequirementStatus.Approved);Assert.Throws<InvalidOperationException>(()=>x.Update(null,"Changed","Description","Criteria",null));}
 private static Requirement Create(string job="JOB-1")=>new(Guid.NewGuid(),null,job,"Title","Description","Criteria",null);
}
