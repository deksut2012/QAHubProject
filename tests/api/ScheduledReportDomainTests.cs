using QAHub.Api.Domain.Reporting;

namespace QAHub.Api.Tests;

public sealed class ScheduledReportDomainTests
{
    [Fact]
    public void DailyScheduleUsesNextDeliveryTime()
    {
        var now = new DateTimeOffset(2026, 7, 21, 10, 0, 0, TimeSpan.Zero);
        var report = new ScheduledReport(null, "Daily QA", ReportFrequency.Daily, new TimeOnly(8, 0), "qa@example.com", now);
        Assert.Equal(new DateTimeOffset(2026, 7, 22, 8, 0, 0, TimeSpan.Zero), report.NextRunAtUtc);
    }

    [Fact]
    public void DisablingAndEnablingRecalculatesNextRun()
    {
        var now = new DateTimeOffset(2026, 7, 21, 10, 0, 0, TimeSpan.Zero);
        var report = new ScheduledReport(null, "Weekly QA", ReportFrequency.Weekly, new TimeOnly(8, 0), "qa@example.com", now);
        report.SetEnabled(false, now); Assert.False(report.IsEnabled);
        report.SetEnabled(true, now.AddDays(8)); Assert.True(report.IsEnabled);
        Assert.True(report.NextRunAtUtc > now.AddDays(8));
    }
}
