using QAHub.Api.Domain.Requirements;
namespace QAHub.Api.Features.Requirements;
public sealed record CreateRequirementRequest(Guid ProductId,Guid? ModuleId,string? JobNumber,string? Title,string? Description,string? AcceptanceCriteria,string? Assignee);
public sealed record UpdateRequirementRequest(Guid? ModuleId,string? Title,string? Description,string? AcceptanceCriteria,string? Assignee);
public sealed record TransitionRequirementRequest(RequirementStatus Status);
public sealed record RequirementResponse(Guid Id,Guid ProductId,Guid? ModuleId,string JobNumber,string Title,string Description,string AcceptanceCriteria,string? Assignee,RequirementStatus Status,DateTimeOffset CreatedAtUtc,DateTimeOffset UpdatedAtUtc);
public sealed record RequirementCollectionResponse(IReadOnlyList<RequirementResponse> Items,int Page,int PageSize,int TotalCount);
