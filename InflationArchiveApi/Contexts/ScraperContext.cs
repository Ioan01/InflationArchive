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
        // Unique index because (UserName, Email) is unique
        // and a query verifying this pair is always used when creating a new user
        modelBuilder.Entity<User>()
            .HasIndex(static e => new { e.Email, e.UserName })
            .IsUnique();


        // Model the many-to-many relationship for Users <-> Products
        // The ORM will create the third table and indexes automatically
        modelBuilder.Entity<Product>()
            .HasMany(static p => p.FavoritedByUsers)
            .WithMany(static u => u.FavoriteProducts)
            .UsingEntity<Dictionary<string, object>>(
                static p => p.HasOne<User>().WithMany().HasForeignKey("UserId"),
                static u => u.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                static j => j.ToTable("FavoritedProducts")
            );


        // Unique index because (Name, Unit, ManufacturerId, CategoryId, StoreId) is unique
        // NOTE: The query verifying this tuple actually only uses (Name and Unit) from product!
        //       The ManufacturerName, CategoryName and StoreName are compared against the
        //       joined tables.
        modelBuilder.Entity<Product>()
            .HasIndex(static e => new
            {
                e.Name,
                e.Unit,
                e.ManufacturerId,
                e.CategoryId,
                e.StoreId
            })
            .IsUnique();


        // Index to speed up GetProducts' filtering using WHERE clause
        modelBuilder.Entity<Product>()
            .HasIndex(static e => new
            {
                e.Name,
                e.PricePerUnit
            });


        // Index to speed up ordering by Name technically
        // already exists because Name is already the first
        // element in the previous 2 indexes


        // Index to speed up ordering by PricePerUnit
        modelBuilder.Entity<Product>()
            .HasIndex(static e => e.PricePerUnit);


        // Unique index because Name is unique
        // and we do a query on this column every
        // time we want to retrieve a store's reference
        modelBuilder.Entity<Store>()
            .HasIndex(static e => e.Name)
            .IsUnique();


        // Unique index because Name is unique
        // and we do a query on this column every
        // time we want to retrieve a manufacturer's reference
        modelBuilder.Entity<Manufacturer>()
            .HasIndex(static e => e.Name)
            .IsUnique();


        // Unique index because Name is unique
        // and we do a query on this column every
        // time we want to retrieve a category's reference
        modelBuilder.Entity<Category>()
            .HasIndex(static e => e.Name)
            .IsUnique();

        // Unique index because (ProductId, Date) is unique
        modelBuilder.Entity<ProductPrice>()
            .HasIndex(static e => new { e.ProductId, e.Date })
            .IsUnique();
    }
}