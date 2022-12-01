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
        this.Database.EnsureCreated();
    }
}