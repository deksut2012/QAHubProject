using QAHub.Api.Domain.Releases;
namespace QAHub.Api.Features.Releases;
public sealed record CreateReleaseRequest(Guid ProductId,Guid BuildId,Guid EnvironmentId,Guid? TestCycleId,string? Name,DateOnly TargetDate,string? ReleaseNotes,string? RollbackPlan,IReadOnlyList<Guid>? RequirementIds,IReadOnlyList<Guid>? KnownIssueBugIds,string? KnownIssueMitigation);
public sealed record UpdateChecklistRequest(bool IsCompleted);
public sealed record SignOffRequest(SignOffDecision Decision,string? Reason);
public sealed record RecordDeploymentRequest(DeploymentStatus Status,string? Notes,bool PostReleaseValidated);
public sealed record GateResponse(string Code,string Label,bool Passed,string Detail,bool Critical);
public sealed record ChecklistResponse(Guid Id,string Code,string Label,bool IsRequired,bool IsCompleted,string CompletedBy,DateTimeOffset? CompletedAtUtc);
public sealed record ReleaseResponse(Guid Id,Guid ProductId,Guid BuildId,Guid EnvironmentId,Guid? TestCycleId,string Name,DateOnly TargetDate,string ReleaseNotes,string RollbackPlan,ReleaseStatus Status,SignOffDecision? Decision,string SignOffBy,string SignOffReason,DeploymentStatus DeploymentStatus,string DeploymentNotes,bool PostReleaseValidated,int ReadinessScore,bool CanApprove,IReadOnlyList<Guid> RequirementIds,IReadOnlyList<Guid> KnownIssueBugIds,IReadOnlyList<GateResponse> Gates,IReadOnlyList<ChecklistResponse> Checklist);
