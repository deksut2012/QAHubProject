using QAHub.Api.Domain.Execution;
using QAHub.Api.Domain.TestDesign;

namespace QAHub.Api.Features.Execution;

public sealed record CreateBuildRequest(Guid ProductId, string? Version);
public sealed record BuildResponse(Guid Id, Guid ProductId, string Version, DateTimeOffset CreatedAtUtc);
public sealed record ExecutionCandidateResponse(Guid TestCaseVersionId, Guid TestCaseId, string Code, string Title, int VersionNumber, TestCaseStatus Status);
public sealed record CreateCycleRequest(Guid ProductId, Guid EnvironmentId, Guid? BuildId, string? Name, string? Assignee, IReadOnlyList<Guid>? TestCaseVersionIds);
public sealed record ExecuteItemRequest(ExecutionResult Result, string? ActualResult, string? Evidence, string? Reason);
public sealed record CreateEvidenceRequest(string? FileName, string? ContentType, string? ContentBase64);
public sealed record EvidenceResponse(Guid Id, string FileName, string ContentType, long SizeBytes, string UploadedBy, DateTimeOffset UploadedAtUtc);
public sealed record AttemptResponse(Guid Id, int AttemptNumber, ExecutionResult Result, string ActualResult, string Evidence, string Reason, string ExecutedBy, DateTimeOffset ExecutedAtUtc, IReadOnlyList<EvidenceResponse> EvidenceFiles);
public sealed record CycleItemResponse(Guid Id, Guid TestCaseVersionId, string TestCaseCode, string Title, int VersionNumber, string Assignee, ExecutionResult Result, int AttemptCount, IReadOnlyList<AttemptResponse> Attempts);
public sealed record CycleResponse(Guid Id, Guid ProductId, Guid EnvironmentId, Guid? BuildId, string Name, string Assignee, TestCycleStatus Status, DateTimeOffset CreatedAtUtc, DateTimeOffset? StartedAtUtc, DateTimeOffset? CompletedAtUtc, IReadOnlyList<CycleItemResponse> Items, int Total, int Executed, int Passed, int Failed, int Blocked, int Skipped);
