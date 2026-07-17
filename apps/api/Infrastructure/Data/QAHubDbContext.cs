using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QAHub.Api.Domain.Auditing;
using QAHub.Api.Domain.Identity;
using QAHub.Api.Domain.Products;
using QAHub.Api.Domain.Requirements;

namespace QAHub.Api.Infrastructure.Data;

public sealed class QAHubDbContext(
    DbContextOptions<QAHubDbContext> options,
    IHttpContextAccessor? httpContextAccessor = null) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductModule> ProductModules => Set<ProductModule>();
    public DbSet<ProductEnvironment> ProductEnvironments => Set<ProductEnvironment>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<AppRole> Roles => Set<AppRole>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Requirement> Requirements => Set<Requirement>();
    public DbSet<RequirementComment> RequirementComments => Set<RequirementComment>();
    public DbSet<RequirementAttachment> RequirementAttachments => Set<RequirementAttachment>();

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

        var user = modelBuilder.Entity<UserAccount>();
        user.ToTable("Users"); user.HasKey(x => x.Id);
        user.Property(x => x.ExternalId).HasMaxLength(200).IsRequired();
        user.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        user.Property(x => x.Email).HasMaxLength(320);
        user.HasIndex(x => x.ExternalId).IsUnique();

        var role = modelBuilder.Entity<AppRole>();
        role.ToTable("Roles"); role.HasKey(x => x.Id);
        role.Property(x => x.Code).HasMaxLength(100).IsRequired();
        role.Property(x => x.Name).HasMaxLength(200).IsRequired();
        role.HasIndex(x => x.Code).IsUnique();

        var userRole = modelBuilder.Entity<UserRole>();
        userRole.ToTable("UserRoles"); userRole.HasKey(x => new { x.UserId, x.RoleId });
        userRole.HasOne(x => x.User).WithMany(x => x.Roles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        userRole.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);

        var requirement = modelBuilder.Entity<Requirement>();
        requirement.ToTable("Requirements"); requirement.HasKey(x => x.Id);
        requirement.Property(x => x.JobNumber).HasMaxLength(50).IsRequired();
        requirement.Property(x => x.Title).HasMaxLength(250).IsRequired();
        requirement.Property(x => x.Description).HasColumnType("nvarchar(max)").IsRequired();
        requirement.Property(x => x.AcceptanceCriteria).HasColumnType("nvarchar(max)").IsRequired();
        requirement.Property(x => x.Assignee).HasMaxLength(200);
        requirement.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        requirement.Property(x => x.RowVersion).IsRowVersion();
        requirement.HasIndex(x => new { x.ProductId, x.JobNumber }).IsUnique();
        requirement.HasIndex(x => new { x.ProductId, x.Status, x.UpdatedAtUtc });
        requirement.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        requirement.HasOne<ProductModule>().WithMany().HasForeignKey(x => x.ModuleId).OnDelete(DeleteBehavior.Restrict);

        var comment = modelBuilder.Entity<RequirementComment>();
        comment.ToTable("RequirementComments"); comment.HasKey(x => x.Id);
        comment.Property(x => x.AuthorId).HasMaxLength(200).IsRequired();
        comment.Property(x => x.Body).HasMaxLength(4000).IsRequired();
        comment.HasIndex(x => new { x.RequirementId, x.CreatedAtUtc });
        comment.HasOne<Requirement>().WithMany().HasForeignKey(x => x.RequirementId).OnDelete(DeleteBehavior.Cascade);

        var attachment = modelBuilder.Entity<RequirementAttachment>();
        attachment.ToTable("RequirementAttachments"); attachment.HasKey(x => x.Id);
        attachment.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        attachment.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
        attachment.Property(x => x.Content).HasColumnType("varbinary(max)").IsRequired();
        attachment.Property(x => x.UploadedBy).HasMaxLength(200).IsRequired();
        attachment.HasIndex(x => new { x.RequirementId, x.UploadedAtUtc });
        attachment.HasOne<Requirement>().WithMany().HasForeignKey(x => x.RequirementId).OnDelete(DeleteBehavior.Cascade);
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
        entry.Entity is Product or ProductModule or ProductEnvironment or UserAccount or AppRole or UserRole or Requirement or RequirementComment or RequirementAttachment;

    private AuditEvent CreateAuditEvent(EntityEntry entry)
    {
        var context = httpContextAccessor?.HttpContext;
        var actorId = context?.User.Identity?.Name ?? "anonymous";
        var correlationId = context?.TraceIdentifier ?? "background";
        var entityId = entry.Entity is UserRole assignment
            ? assignment.UserId
            : (Guid)(entry.Property("Id").CurrentValue ?? Guid.Empty);
        Guid? productId = entry.Entity switch
        {
            Product product => product.Id,
            ProductModule module => module.ProductId,
            ProductEnvironment environment => environment.ProductId,
            Requirement requirement => requirement.ProductId,
            RequirementComment comment => Requirements.Where(x => x.Id == comment.RequirementId).Select(x => (Guid?)x.ProductId).FirstOrDefault(),
            RequirementAttachment attachment => Requirements.Where(x => x.Id == attachment.RequirementId).Select(x => (Guid?)x.ProductId).FirstOrDefault(),
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
