using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace {{ProjectName}}.Server.Data;
    public class AppDbContext : IdentityDbContext<ApplicationUser>
{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Sample DbSet - replace with your actual entities
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure your entities here
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });
        }
    }

