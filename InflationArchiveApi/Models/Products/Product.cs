using System.ComponentModel.DataAnnotations.Schema;
using InflationArchive.Helpers;

namespace InflationArchive.Models.Products;

public sealed class Product : ScraperEntity
{
    public decimal PricePerUnit { get; set; }
    public string Unit { get; set; }
    public bool IsUnreliable { get; set; }

    public ICollection<ProductPrice> ProductPrices { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

    [NotMapped] internal readonly string CategoryName;

    public Guid ManufacturerId { get; set; }
    public Manufacturer Manufacturer { get; set; }

    [NotMapped] internal readonly string ManufacturerName;

    public Guid StoreId { get; set; }
    public Store Store { get; set; }

    [NotMapped] internal readonly string StoreName;

    public Product()
    {
    }

    public Product(string name, string? imageUri, decimal pricePerUnit, string unit, Category category,
        Manufacturer manufacturer, Store store)
    {
        Id = Guid.NewGuid();

        Name = name.OnlyFirstCharToUpper();
        ImageUri = imageUri;
        PricePerUnit = Convert.ToDecimal(Math.Round(pricePerUnit, 2));
        Unit = unit.OnlyFirstCharToUpper();

        CategoryId = category.Id;
        CategoryName = category.Name;

        ManufacturerId = manufacturer.Id;
        ManufacturerName = manufacturer.Name;

        StoreId = store.Id;
        StoreName = store.Name;
    }

    private bool Equals(Product other)
    {
        return Name == other.Name && CategoryName == other.CategoryName && ManufacturerName == other.ManufacturerName &&
               StoreName == other.StoreName && Unit == other.Unit;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is Product other && Equals(other));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, CategoryName, ManufacturerName, StoreName, Unit);
    }
}