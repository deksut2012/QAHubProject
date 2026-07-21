namespace QAHub.Api.Features.Reporting;
public sealed record NamedCount(string Name,int Count);
public sealed record ExecutionSummary(int Total,int Executed,int Passed,int Failed,int Blocked,int Skipped,double PassRate);
public sealed record TrendPoint(DateOnly Date,int Passed,int Failed,int Blocked);
public sealed record WorkloadItem(string Assignee,int TestItems,int OpenBugs);
public sealed record DashboardResponse(int ActiveProducts,int Requirements,int TestCases,int OpenBugs,int CriticalHighBugs,double RequirementCoverage,ExecutionSummary Execution,IReadOnlyList<NamedCount> RequirementStatuses,IReadOnlyList<NamedCount> BugSeverities,IReadOnlyList<TrendPoint> ExecutionTrend,IReadOnlyList<WorkloadItem> Workload,int ReleaseCount,double AverageReleaseReadiness);
public static class DashboardCalculator{public static double Percentage(int numerator,int denominator)=>denominator==0?0:Math.Round(numerator*100d/denominator,1);}
