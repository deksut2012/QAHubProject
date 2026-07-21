namespace QAHub.Api.Domain.Releases;
public sealed class ReleaseRequirement{private ReleaseRequirement(){}public ReleaseRequirement(Guid releaseId,Guid requirementId){Id=Guid.NewGuid();ReleaseId=releaseId;RequirementId=requirementId;}public Guid Id{get;private set;}public Guid ReleaseId{get;private set;}public Guid RequirementId{get;private set;}}
