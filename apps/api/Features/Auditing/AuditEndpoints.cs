using Microsoft.EntityFrameworkCore;
using QAHub.Api.Infrastructure.Data;
using QAHub.Api.Infrastructure.Security;

namespace QAHub.Api.Features.Auditing;

public static class AuditEndpoints
{
    public static IEndpointRouteBuilder MapAuditEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/audit-events", GetAuditEvents)
            .WithTags("Audit")
            .RequireAuthorization(AuthorizationPolicies.AuditRead);
        return endpoints;
    }

    private static async Task<IResult> GetAuditEvents(
        QAHubDbContext db,
        Guid? productId,
        string? entityType,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        var query = db.AuditEvents.AsNoTracking();
        if (productId.HasValue) query = query.Where(x => x.ProductId == productId);
        if (!string.IsNullOrWhiteSpace(entityType)) query = query.Where(x => x.EntityType == entityType.Trim());
        var items = await query.OrderByDescending(x => x.OccurredAtUtc).Take(Math.Clamp(take, 1, 500))
            .Select(x => new { x.Id, x.OccurredAtUtc, x.ActorId, x.Action, x.EntityType, x.EntityId, x.ProductId, x.CorrelationId, x.ChangesJson })
            .ToListAsync(cancellationToken);
        return TypedResults.Ok(new { items, totalCount = items.Count });
    }
}
