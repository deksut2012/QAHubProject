using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QAHub.Api.Domain.Auditing;
using QAHub.Api.Domain.Products;

namespace QAHub.Api.Infrastructure.Data;

public sealed class QAHubDbContext(
    DbContextOptions<QAHubDbContext> options,
    IHttpContextAccessor? httpContextAccessor = null) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductModule> ProductModules => Set<ProductModule>();
    public DbSet<ProductEnvironment> ProductEnvironments => Set<ProductEnvironment>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var product = modelBuilder.Entity<Product>();
        product.ToTable("Products");
        product.HasKey(x => x.Id);
        product.Property(x => x.Code).HasMaxLength(32).IsRequired();
        product.Property(x => x.Name).HasMaxLength(200).IsRequired();
        product.Property(x => x.RowVersion).IsRowVersion();
        product.HasIndex(x => x.Code).IsUnique();

        var module = modelBuilder.Entity<ProductModule>();
        module.ToTable("ProductModules");
        module.HasKey(x => x.Id);
        module.Property(x => x.Code).HasMaxLength(32).IsRequired();
        module.Property(x => x.Name).HasMaxLength(200).IsRequired();
        module.HasIndex(x => new { x.ProductId, x.Code }).IsUnique();
        module.HasOne(x => x.Product)
            .WithMany(x => x.Modules)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        var environment = modelBuilder.Entity<ProductEnvironment>();
        environment.ToTable("ProductEnvironments");
        environment.HasKey(x => x.Id);
        environment.Property(x => x.Code).HasMaxLength(32).IsRequired();
        environment.Property(x => x.Name).HasMaxLength(200).IsRequired();
        environment.HasIndex(x => new { x.ProductId, x.Code }).IsUnique();
        environment.HasOne(x => x.Product)
            .WithMany(x => x.Environments)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        var audit = modelBuilder.Entity<AuditEvent>();
        audit.ToTable("AuditEvents");
        audit.HasKey(x => x.Id);
        audit.Property(x => x.ActorId).HasMaxLength(200).IsRequired();
        audit.Property(x => x.Action).HasMaxLength(100).IsRequired();
        audit.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
        audit.Property(x => x.CorrelationId).HasMaxLength(200).IsRequired();
        audit.Property(x => x.ChangesJson).HasColumnType("nvarchar(max)").IsRequired();
        audit.HasIndex(x => new { x.EntityType, x.EntityId, x.OccurredAtUtc });
        audit.HasIndex(x => new { x.ProductId, x.OccurredAtUtc });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var events = ChangeTracker.Entries()
            .Where(IsAuditableChange)
            .Select(CreateAuditEvent)
            .ToArray();
        AuditEvents.AddRange(events);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private static bool IsAuditableChange(EntityEntry entry) =>
        entry.State is EntityState.Added or EntityState.Modified &&
        entry.Entity is Product or ProductModule or ProductEnvironment;

    private AuditEvent CreateAuditEvent(EntityEntry entry)
    {
        var context = httpContextAccessor?.HttpContext;
        var actorId = context?.User.Identity?.Name ?? "anonymous";
        var correlationId = context?.TraceIdentifier ?? "background";
        var entityId = (Guid)(entry.Property("Id").CurrentValue ?? Guid.Empty);
        Guid? productId = entry.Entity switch
        {
            Product product => product.Id,
            ProductModule module => module.ProductId,
            ProductEnvironment environment => environment.ProductId,
            _ => (Guid?)null,
        };
        var changes = entry.Properties
            .Where(property => entry.State == EntityState.Added || property.IsModified)
            .Where(property => property.Metadata.Name is not "RowVersion")
            .ToDictionary(
                property => property.Metadata.Name,
                property => new { before = entry.State == EntityState.Added ? null : property.OriginalValue, after = property.CurrentValue });

        return new AuditEvent(
            actorId,
            entry.State == EntityState.Added ? "created" : "updated",
            entry.Metadata.ClrType.Name,
            entityId,
            productId,
            correlationId,
            JsonSerializer.Serialize(changes));
    }
}
