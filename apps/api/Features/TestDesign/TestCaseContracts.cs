using QAHub.Api.Domain.TestDesign;namespace QAHub.Api.Features.TestDesign;
public sealed record TestStepRequest(string? Action,string? TestData,string? ExpectedResult);
public sealed record CreateTestCaseRequest(Guid ProductId,Guid? ModuleId,Guid? RequirementId,string? Code,string? Title,string? Scenario,string? Preconditions,string? Tags,IReadOnlyList<TestStepRequest>? Steps);
public sealed record UpdateTestCaseVersionRequest(string? Title,string? Scenario,string? Preconditions,string? Tags,IReadOnlyList<TestStepRequest>? Steps);
public sealed record TransitionTestCaseRequest(TestCaseStatus Status);
public sealed record TestStepResponse(Guid Id,int Sequence,string Action,string TestData,string ExpectedResult);
public sealed record TestCaseResponse(Guid Id,Guid ProductId,Guid? ModuleId,Guid? RequirementId,string Code,int Version,Guid VersionId,string Title,string Scenario,string Preconditions,string Tags,TestCaseStatus Status,IReadOnlyList<TestStepResponse> Steps,DateTimeOffset UpdatedAtUtc);
public sealed record TestCaseListItem(Guid Id,Guid ProductId,Guid? ModuleId,Guid? RequirementId,string Code,int Version,string Title,TestCaseStatus Status,DateTimeOffset UpdatedAtUtc);
public sealed record TestCaseCollectionResponse(IReadOnlyList<TestCaseListItem> Items,int Page,int PageSize,int TotalCount);
