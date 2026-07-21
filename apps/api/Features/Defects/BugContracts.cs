using QAHub.Api.Domain.Defects;
namespace QAHub.Api.Features.Defects;
public sealed record CreateBugRequest(Guid ProductId,string? Code,string? Title,string? Description,string? StepsToReproduce,string? ExpectedResult,string? ActualResult,BugSeverity Severity,BugPriority Priority,string? Assignee,IReadOnlyList<Guid>? TestRunAttemptIds);
public sealed record TransitionBugRequest(BugStatus Status,string? Reason,string? Assignee,Guid? FixBuildId,Guid? CanonicalBugId);
public sealed record BugHistoryResponse(BugStatus FromStatus,BugStatus ToStatus,string ActorId,string Reason,DateTimeOffset ChangedAtUtc);
public sealed record BugResponse(Guid Id,Guid ProductId,string Code,string Title,string Description,string StepsToReproduce,string ExpectedResult,string ActualResult,BugSeverity Severity,BugPriority Priority,BugStatus Status,string Reporter,string Assignee,Guid? FixBuildId,Guid? CanonicalBugId,DateTimeOffset CreatedAtUtc,DateTimeOffset UpdatedAtUtc,int AgingDays,IReadOnlyList<Guid> TestRunAttemptIds,IReadOnlyList<BugHistoryResponse> History);
