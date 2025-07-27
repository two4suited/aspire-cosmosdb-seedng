using cosmosdb_seeding.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace cosmosdb_seeding.ApiService.Data;

/// <summary>
/// Entity Framework DbContext for Cosmos DB seeding operations.
/// </summary>
public class SeedingDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeedingDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public SeedingDbContext(DbContextOptions<SeedingDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the seed items collection.
    /// </summary>
    public DbSet<SeedItem> SeedItems => Set<SeedItem>();

    /// <summary>
    /// Configures the model for the context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the SeedItem entity
        modelBuilder.Entity<SeedItem>(entity =>
        {
            // Set the container name
            entity.ToContainer("items");

            // Configure the partition key
            entity.HasPartitionKey(e => e.Id);

            // Configure properties
            entity.Property(e => e.Id)
                .IsRequired();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.IsActive)
                .IsRequired();

            // Configure the Metadata property as a complex type
            entity.OwnsOne(e => e.Metadata, metadata =>
            {
                metadata.ToJsonProperty("metadata");
            });
        });
    }
}
