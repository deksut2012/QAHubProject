using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace QAHub.Api.Infrastructure.Security;

public static class SecurityExtensions
{
    public static IServiceCollection AddQAHubSecurity(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddAuthentication(DevelopmentAuthenticationHandler.SchemeName)
                .AddScheme<DevelopmentAuthenticationOptions, DevelopmentAuthenticationHandler>(
                    DevelopmentAuthenticationHandler.SchemeName,
                    _ => { });
        }
        else
        {
            var authority = configuration["Authentication:Authority"]
                ?? throw new InvalidOperationException("Authentication:Authority is required outside Development.");
            var audience = configuration["Authentication:Audience"]
                ?? throw new InvalidOperationException("Authentication:Audience is required outside Development.");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = audience;
                    options.RequireHttpsMetadata = true;
                });
        }

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.ProductAccess, policy =>
                policy.RequireRole("SystemAdmin", "ProductOwner", "QALead", "QAEngineer", "Developer", "ReleaseManager", "Auditor"))
            .AddPolicy(AuthorizationPolicies.Administration, policy =>
                policy.RequireRole("SystemAdmin"))
            .AddPolicy(AuthorizationPolicies.AuditRead, policy =>
                policy.RequireRole("SystemAdmin", "QALead", "Auditor"))
            .AddPolicy(AuthorizationPolicies.ToolboxUse, policy =>
                policy.RequireRole("SystemAdmin", "QALead", "QAEngineer", "Developer"));
        return services;
    }
}
