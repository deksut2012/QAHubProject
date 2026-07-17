namespace QAHub.Api.Domain.TestDesign;
public sealed class TestCase
{
 private TestCase(){}
 public TestCase(Guid productId,Guid? moduleId,Guid? requirementId,string code){Id=Guid.NewGuid();ProductId=productId;ModuleId=moduleId;RequirementId=requirementId;Code=code.Trim().ToUpperInvariant();CreatedAtUtc=UpdatedAtUtc=DateTimeOffset.UtcNow;}
 public Guid Id{get;private set;} public Guid ProductId{get;private set;} public Guid? ModuleId{get;private set;} public Guid? RequirementId{get;private set;} public string Code{get;private set;}=string.Empty;public int CurrentVersionNumber{get;private set;}=1;public DateTimeOffset CreatedAtUtc{get;private set;}public DateTimeOffset UpdatedAtUtc{get;private set;}public byte[] RowVersion{get;private set;}=[];public List<TestCaseVersion> Versions{get;private set;}=[];
 public void AdvanceVersion(){CurrentVersionNumber++;UpdatedAtUtc=DateTimeOffset.UtcNow;}
}
