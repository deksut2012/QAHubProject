using QAHub.Api.Infrastructure.Security;
namespace QAHub.Api.Features.Toolbox;
public sealed record ValidateSqlRequest(string Statement);
public static class ToolboxEndpoints
{
 public static IEndpointRouteBuilder MapToolboxEndpoints(this IEndpointRouteBuilder endpoints){var group=endpoints.MapGroup("/api/v1/toolbox").WithTags("Toolbox").RequireAuthorization(AuthorizationPolicies.ProductAccess);group.MapPost("/sql/validate",(ValidateSqlRequest request)=>Results.Ok(SqlSafetyPolicy.Validate(request.Statement)));return endpoints;}
}
