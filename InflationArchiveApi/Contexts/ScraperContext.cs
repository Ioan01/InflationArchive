using InflationArchive.Models.Account;
using InflationArchive.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Contexts;

public class ScraperContext : DbContext
{
    public DbSet<User> Users { get; set; }
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
        modelBuilder.Entity<User>()
            .HasIndex(static e => new { e.UserName, e.Email })
            .IsUnique();


        modelBuilder.Entity<Product>()
            .HasMany(static p => p.FavoritedByUsers)
            .WithMany(static u => u.FavoriteProducts)
            .UsingEntity<Dictionary<string, object>>(
                static p => p.HasOne<User>().WithMany().HasForeignKey("UserId"),
                static u => u.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                static j => j.ToTable("FavoritedProducts")
            );


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