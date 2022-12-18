using InflationArchive.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Contexts;

public class ScraperContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<ProductPrice> ProductPrices { get; set; }
    public DbSet<Category> Categories { get; set; }

    public ScraperContext(DbContextOptions<ScraperContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(static e => new
            {
                e.Name,
                e.CategoryId,
                e.ManufacturerId,
                e.StoreId,
                e.Unit,
                e.PricePerUnit
            })
            .IsUnique();

        modelBuilder.Entity<Store>()
            .HasIndex(static e => new { e.Name })
            .IsUnique();

        modelBuilder.Entity<Manufacturer>()
            .HasIndex(static e => new { e.Name })
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(static e => new { e.Name })
            .IsUnique();

        modelBuilder.Entity<ProductPrice>()
            .HasIndex(static e => new { e.ProductId, e.Date })
            .IsUnique();
    }
}