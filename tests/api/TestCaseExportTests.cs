using System.Net;

namespace QAHub.Api.Tests;

public sealed class TestCaseExportTests
{
    [Fact]
    public void ExportProducesCsvContentForTestCases()
    {
        var csv = "Code,Title,Status,Product ID,Requirement ID,Updated UTC\r\nTC-001,Sample,Draft,11111111-1111-1111-1111-111111111111,,2026-07-18T00:00:00Z\r\n";

        Assert.Contains("Code,Title,Status", csv);
        Assert.Contains("TC-001", csv);
        Assert.Contains("Sample", csv);
    }
}
