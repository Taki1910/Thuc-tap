using Microsoft.EntityFrameworkCore;
using ThucTap.Server.Models;

namespace ThucTap.Server.Data;

public sealed class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();

    public static void Configure(DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlServer(connectionString)
            .UseSeeding((context, _) => Seed(context))
            .UseAsyncSeeding((context, _, cancellationToken) => SeedAsync(context, cancellationToken));

    private static void Seed(DbContext context)
    {
        var existingIds = context.Set<Product>().Select(product => product.Id).ToHashSet();
        context.Set<Product>().AddRange(SampleProducts.All.Where(product => !existingIds.Contains(product.Id)));
        context.SaveChanges();
    }

    private static async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
    {
        var existingIds = await context.Set<Product>().Select(product => product.Id).ToListAsync(cancellationToken);
        context.Set<Product>().AddRange(SampleProducts.All.Where(product => !existingIds.Contains(product.Id)));
        await context.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Id).HasMaxLength(36);
            entity.Property(product => product.Name).HasMaxLength(160).IsRequired();
            entity.Property(product => product.Category).HasMaxLength(80).IsRequired();
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.Property(product => product.ImageUrl).HasMaxLength(2048).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(4000).IsRequired();
            entity.HasMany(product => product.Reviews)
                .WithOne(review => review.Product)
                .HasForeignKey(review => review.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(review => review.Id);
            entity.Property(review => review.Id).HasMaxLength(36);
            entity.Property(review => review.ProductId).HasMaxLength(36);
            entity.Property(review => review.Author).HasMaxLength(100).IsRequired();
            entity.Property(review => review.Comment).HasMaxLength(2000).IsRequired();
            entity.HasIndex(review => new { review.ProductId, review.Date });
        });
    }
}
