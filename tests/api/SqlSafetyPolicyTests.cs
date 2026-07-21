using QAHub.Api.Features.Toolbox;
namespace QAHub.Api.Tests;
public sealed class SqlSafetyPolicyTests
{
 [Theory][InlineData("SELECT Id, Name FROM Products")][InlineData("WITH Active AS (SELECT Id FROM Products WHERE IsActive = 1) SELECT * FROM Active")]
 public void AllowsSingleReadOnlyStatement(string sql)=>Assert.True(SqlSafetyPolicy.Validate(sql).IsSafe);
 [Theory][InlineData("UPDATE Products SET Name = 'x'")][InlineData("SELECT * INTO ProductCopy FROM Products")][InlineData("SELECT * FROM Products; DELETE FROM Products")][InlineData("EXEC sp_who")][InlineData("SELECT * FROM Products -- bypass")]
 public void BlocksDangerousOrAmbiguousStatement(string sql){var result=SqlSafetyPolicy.Validate(sql);Assert.False(result.IsSafe);Assert.NotEmpty(result.Violations);}
}
