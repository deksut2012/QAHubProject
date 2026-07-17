using Microsoft.EntityFrameworkCore;
using QAHub.Api.Domain.Products;

namespace QAHub.Api.Infrastructure.Data;

public sealed class QAHubDbContext(DbContextOptions<QAHubDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductModule> ProductModules => Set<ProductModule>();

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
    }
}
