namespace QAHub.Api.Infrastructure.Security;

public static class AuthorizationPolicies
{
    public const string ProductAccess = nameof(ProductAccess);
    public const string Administration = nameof(Administration);
    public const string AuditRead = nameof(AuditRead);
}
