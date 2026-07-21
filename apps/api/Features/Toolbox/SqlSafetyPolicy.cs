using System.Text.RegularExpressions;
namespace QAHub.Api.Features.Toolbox;
public sealed record SqlValidationResult(bool IsSafe,string NormalizedStatement,IReadOnlyList<string> Violations);
public static partial class SqlSafetyPolicy
{
 public static SqlValidationResult Validate(string? statement){if(string.IsNullOrWhiteSpace(statement))return new(false,string.Empty,["SQL statement is required."]);var normalized=Whitespace().Replace(statement.Trim()," ");var violations=new List<string>();if(!StartsReadOnly().IsMatch(normalized))violations.Add("Only SELECT or CTE (WITH) statements are allowed.");if(normalized.TrimEnd().TrimEnd(';').Contains(';'))violations.Add("Multiple SQL statements are not allowed.");foreach(Match match in ForbiddenWords().Matches(normalized))violations.Add($"Keyword {match.Value.ToUpperInvariant()} is not allowed.");if(SelectInto().IsMatch(normalized))violations.Add("SELECT INTO is not allowed.");if(normalized.Contains("--",StringComparison.Ordinal)||normalized.Contains("/*",StringComparison.Ordinal))violations.Add("SQL comments are not allowed in Toolbox queries.");return new(violations.Count==0,normalized,violations.Distinct().ToList());}
 [GeneratedRegex(@"\s+")]private static partial Regex Whitespace();
 [GeneratedRegex(@"^(SELECT|WITH)\b",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant)]private static partial Regex StartsReadOnly();
 [GeneratedRegex(@"\b(INSERT|UPDATE|DELETE|MERGE|DROP|ALTER|CREATE|TRUNCATE|EXEC|EXECUTE|GRANT|REVOKE|DENY|BACKUP|RESTORE|DBCC|BULK|OPENROWSET|OPENDATASOURCE|SHUTDOWN|KILL)\b",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant)]private static partial Regex ForbiddenWords();
 [GeneratedRegex(@"\bSELECT\s+.+\s+INTO\b",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant)]private static partial Regex SelectInto();
}
