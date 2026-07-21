using QAHub.Api.Features.Reporting;
namespace QAHub.Api.Tests;
public sealed class DashboardCalculatorTests
{
 [Theory][InlineData(0,0,0)][InlineData(5,10,50)][InlineData(2,3,66.7)]public void PercentageIsStable(int numerator,int denominator,double expected)=>Assert.Equal(expected,DashboardCalculator.Percentage(numerator,denominator));
 [Theory][InlineData(0,"0-7 days")][InlineData(7,"0-7 days")][InlineData(8,"8-14 days")][InlineData(15,"15-30 days")][InlineData(31,"31+ days")]
 public void AgingBandUsesDefinedBoundaries(int days,string expected)=>Assert.Equal(expected,DashboardCalculator.AgingBand(days));
 [Theory][InlineData("Critical",1)][InlineData("High",3)][InlineData("Medium",7)][InlineData("Low",14)]
 public void BugSlaMatchesSeverity(string severity,int expected)=>Assert.Equal(expected,DashboardCalculator.BugSlaDays(severity));

 [Fact]
 public void ReconciliationTotalsMatchLatestTransactionResults()
 {
  QAHub.Api.Domain.Execution.ExecutionResult?[] latest=[QAHub.Api.Domain.Execution.ExecutionResult.Passed,QAHub.Api.Domain.Execution.ExecutionResult.Failed,QAHub.Api.Domain.Execution.ExecutionResult.Blocked,null];
  var result=DashboardCalculator.ReconcileExecution(latest);
  Assert.Equal(4,result.Total);Assert.Equal(3,result.Executed);Assert.Equal(1,result.Passed);Assert.Equal(1,result.Failed);Assert.Equal(1,result.Blocked);Assert.Equal(33.3,result.PassRate);
 }
}
