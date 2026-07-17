namespace QAHub.Api.Domain.TestDesign;
public sealed class TestCaseStep
{
 private TestCaseStep(){}
 public TestCaseStep(Guid versionId,int sequence,string action,string testData,string expectedResult){Id=Guid.NewGuid();TestCaseVersionId=versionId;Sequence=sequence;Action=action.Trim();TestData=testData.Trim();ExpectedResult=expectedResult.Trim();}
 public Guid Id{get;private set;}public Guid TestCaseVersionId{get;private set;}public int Sequence{get;private set;}public string Action{get;private set;}=string.Empty;public string TestData{get;private set;}=string.Empty;public string ExpectedResult{get;private set;}=string.Empty;
}
