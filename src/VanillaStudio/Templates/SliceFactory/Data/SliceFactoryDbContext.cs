using Microsoft.EntityFrameworkCore;
using {{RootNamespace}}.SliceFactory.Models;

namespace {{RootNamespace}}.SliceFactory.Data;

public class SliceFactoryDbContext : DbContext
{
    public SliceFactoryDbContext(DbContextOptions<SliceFactoryDbContext> options) : base(options)
    {
    }

    public DbSet<Feature> Features { get; set; }
    public DbSet<FeatureFile> FeatureFiles { get; set; }
    public DbSet<FeatureProject> FeatureProjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Feature entity
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ComponentPrefix).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModuleNamespace).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProjectNamespace).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PrimaryKeyType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BasePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.DirectoryName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ProfileConfiguration).HasColumnType("TEXT");

            // Index for common queries
            entity.HasIndex(e => e.ModuleNamespace);
            entity.HasIndex(e => e.ComponentPrefix);
            entity.HasIndex(e => new { e.ModuleNamespace, e.ComponentPrefix }).IsUnique();
        });

        // Configure FeatureFile entity
        modelBuilder.Entity<FeatureFile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProjectType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SliceType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();

            // Foreign key relationship
            entity.HasOne(e => e.Feature)
                  .WithMany(f => f.Files)
                  .HasForeignKey(e => e.FeatureId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Index for common queries
            entity.HasIndex(e => e.FeatureId);
            entity.HasIndex(e => e.ProjectType);
            entity.HasIndex(e => e.SliceType);
        });

        // Configure FeatureProject entity
        modelBuilder.Entity<FeatureProject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProjectPath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ProjectNamespace).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();

            // Convert enum to string
            entity.Property(e => e.ProjectType)
                  .HasConversion<string>()
                  .IsRequired();

            // Foreign key relationship
            entity.HasOne(e => e.Feature)
                  .WithMany(f => f.Projects)
                  .HasForeignKey(e => e.FeatureId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Index for common queries
            entity.HasIndex(e => e.FeatureId);
            entity.HasIndex(e => e.ProjectType);

            // Unique constraint to prevent duplicate project types per feature
            entity.HasIndex(e => new { e.FeatureId, e.ProjectType }).IsUnique();
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback configuration if not configured in DI
            optionsBuilder.UseSqlite("Data Source=slicefactory.db");
        }
    }
}
