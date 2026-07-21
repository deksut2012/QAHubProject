namespace QAHub.Api.Features.Reporting;

public sealed record NamedCount(string Name, int Count);
public sealed record ExecutionSummary(int Total, int Executed, int Passed, int Failed, int Blocked, int Skipped, double PassRate);
public sealed record TrendPoint(DateOnly Date, int Passed, int Failed, int Blocked);
public sealed record WorkloadItem(string Assignee, int TestItems, int OpenBugs);
public sealed record BugAgingSummary(int WithinSla, int Breached, double AverageAgeDays, IReadOnlyList<NamedCount> Bands);
public sealed record ReleaseReadinessItem(Guid Id, string Name, string Status, DateOnly TargetDate, int Completed, int Required, double Score);
public sealed record DashboardResponse(
    int ActiveProducts,
    int Requirements,
    int TestCases,
    int OpenBugs,
    int CriticalHighBugs,
    double RequirementCoverage,
    ExecutionSummary Execution,
    IReadOnlyList<NamedCount> RequirementStatuses,
    IReadOnlyList<NamedCount> BugSeverities,
    IReadOnlyList<TrendPoint> ExecutionTrend,
    IReadOnlyList<WorkloadItem> Workload,
    BugAgingSummary BugAging,
    int ReleaseCount,
    double AverageReleaseReadiness,
    IReadOnlyList<ReleaseReadinessItem> ReleaseReadiness);

public static class DashboardCalculator
{
    public static double Percentage(int numerator, int denominator) =>
        denominator == 0 ? 0 : Math.Round(numerator * 100d / denominator, 1);

    public static string AgingBand(int days) => days switch
    {
        <= 7 => "0-7 days",
        <= 14 => "8-14 days",
        <= 30 => "15-30 days",
        _ => "31+ days"
    };

    public static int BugSlaDays(string severity) => severity switch
    {
        "Critical" => 1,
        "High" => 3,
        "Medium" => 7,
        _ => 14
    };
}
