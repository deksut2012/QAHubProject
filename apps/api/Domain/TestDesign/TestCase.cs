namespace QAHub.Api.Domain.TestDesign;
public sealed class TestCase
{
 private TestCase(){}
 public TestCase(Guid productId,Guid? moduleId,Guid? requirementId,string code){Id=Guid.NewGuid();ProductId=productId;ModuleId=moduleId;RequirementId=requirementId;Code=code.Trim().ToUpperInvariant();CreatedAtUtc=UpdatedAtUtc=DateTimeOffset.UtcNow;}
 public Guid Id{get;private set;} public Guid ProductId{get;private set;} public Guid? ModuleId{get;private set;} public Guid? RequirementId{get;private set;} public string Code{get;private set;}=string.Empty;public int CurrentVersionNumber{get;private set;}=1;public DateTimeOffset CreatedAtUtc{get;private set;}public DateTimeOffset UpdatedAtUtc{get;private set;}public byte[] RowVersion{get;private set;}=[];public Guid? SourceTestCaseId{get;private set;}public Guid? SourceVersionId{get;private set;}public List<TestCaseVersion> Versions{get;private set;}=[];
 public TestCase CloneWithVersion(TestCaseVersion sourceVersion,string code){var clone=new TestCase(ProductId,ModuleId,RequirementId,code);clone.SourceTestCaseId=Id;clone.SourceVersionId=sourceVersion.Id;var version=new TestCaseVersion(clone.Id,1,sourceVersion.Title,sourceVersion.Scenario,sourceVersion.Preconditions,sourceVersion.Tags);foreach(var step in sourceVersion.Steps.OrderBy(x=>x.Sequence)){version.Steps.Add(new TestCaseStep(version.Id,step.Sequence,step.Action,step.TestData,step.ExpectedResult));}clone.Versions.Add(version);return clone;}
 public void AdvanceVersion(){CurrentVersionNumber++;UpdatedAtUtc=DateTimeOffset.UtcNow;}
 public void UpdateCoverage(Guid? requirementId,Guid? moduleId){RequirementId=requirementId;ModuleId=moduleId;UpdatedAtUtc=DateTimeOffset.UtcNow;}
}
