using QAHub.Api.Features.Reporting;
namespace QAHub.Api.Tests;
public sealed class DashboardCalculatorTests
{
 [Theory][InlineData(0,0,0)][InlineData(5,10,50)][InlineData(2,3,66.7)]public void PercentageIsStable(int numerator,int denominator,double expected)=>Assert.Equal(expected,DashboardCalculator.Percentage(numerator,denominator));
}
