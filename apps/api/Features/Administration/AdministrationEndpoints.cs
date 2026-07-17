using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Identity;
using QAHub.Api.Infrastructure.Data;
using QAHub.Api.Infrastructure.Security;

namespace QAHub.Api.Features.Administration;

public sealed record CreateUserRequest(string? ExternalId, string? DisplayName, string? Email);
public sealed record CreateRoleRequest(string? Code, string? Name);
public sealed record AssignRoleRequest(Guid RoleId);

public static class AdministrationEndpoints
{
    public static IEndpointRouteBuilder MapAdministrationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/admin").WithTags("Administration")
            .RequireAuthorization(AuthorizationPolicies.Administration);
        group.MapGet("/users", async (QAHubDbContext db, CancellationToken token) =>
            TypedResults.Ok(await db.Users.AsNoTracking().OrderBy(x => x.DisplayName)
                .Select(x => new { x.Id, x.ExternalId, x.DisplayName, x.Email, x.IsActive, roles = x.Roles.Select(r => r.Role.Code) })
                .ToListAsync(token)));
        group.MapPost("/users", CreateUser);
        group.MapGet("/roles", async (QAHubDbContext db, CancellationToken token) =>
            TypedResults.Ok(await db.Roles.AsNoTracking().OrderBy(x => x.Code).ToListAsync(token)));
        group.MapPost("/roles", CreateRole);
        group.MapPost("/users/{userId:guid}/roles", AssignRole);
        return endpoints;
    }

    private static async Task<IResult> CreateUser(CreateUserRequest request, QAHubDbContext db, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(request.ExternalId) || string.IsNullOrWhiteSpace(request.DisplayName))
            return TypedResults.ValidationProblem(new Dictionary<string, string[]> { ["user"] = ["External ID and display name are required."] });
        if (await db.Users.AnyAsync(x => x.ExternalId == request.ExternalId.Trim(), token)) return TypedResults.Conflict();
        var user = new UserAccount(request.ExternalId, request.DisplayName, request.Email);
        db.Users.Add(user); await db.SaveChangesAsync(token);
        return TypedResults.Created($"/api/v1/admin/users/{user.Id}", new { user.Id, user.ExternalId, user.DisplayName, user.Email, user.IsActive });
    }

    private static async Task<IResult> CreateRole(CreateRoleRequest request, QAHubDbContext db, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
            return TypedResults.ValidationProblem(new Dictionary<string, string[]> { ["role"] = ["Code and name are required."] });
        if (await db.Roles.AnyAsync(x => x.Code == request.Code.Trim(), token)) return TypedResults.Conflict();
        var role = new AppRole(request.Code, request.Name);
        db.Roles.Add(role); await db.SaveChangesAsync(token);
        return TypedResults.Created($"/api/v1/admin/roles/{role.Id}", new { role.Id, role.Code, role.Name });
    }

    private static async Task<IResult> AssignRole(Guid userId, AssignRoleRequest request, QAHubDbContext db, CancellationToken token)
    {
        if (!await db.Users.AnyAsync(x => x.Id == userId, token) || !await db.Roles.AnyAsync(x => x.Id == request.RoleId, token)) return TypedResults.NotFound();
        if (await db.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == request.RoleId, token)) return TypedResults.NoContent();
        db.UserRoles.Add(new UserRole(userId, request.RoleId)); await db.SaveChangesAsync(token);
        return TypedResults.NoContent();
    }
}
