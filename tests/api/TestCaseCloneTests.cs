using QAHub.Api.Domain.TestDesign;

namespace QAHub.Api.Tests;

public sealed class TestCaseCloneTests
{
    [Fact]
    public void CloneWithVersionCopiesContentIntoNewTestCase()
    {
        var source = new TestCase(Guid.NewGuid(), null, null, "TC-001");
        var version = new TestCaseVersion(source.Id, 1, "Original title", "Scenario", "Preconditions", "smoke");
        version.Steps.Add(new TestCaseStep(version.Id, 1, "Open app", "Input", "Result"));

        var clone = source.CloneWithVersion(version, "TC-002");

        Assert.Equal("TC-002", clone.Code);
        Assert.Single(clone.Versions);
        Assert.Equal("Original title", clone.Versions[0].Title);
        Assert.Single(clone.Versions[0].Steps);
        Assert.Equal("Open app", clone.Versions[0].Steps[0].Action);
    }
}
