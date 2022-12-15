namespace InflationArchive.Models.Products;

public class Product : ScraperEntity
{
    public decimal PricePerUnit { get; set; }
    public string Unit { get; set; }

    public IEnumerable<ProductPrice> Prices;

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public int ManufacturerId { get; set; }
    public Manufacturer Manufacturer { get; set; }

    public int StoreId { get; set; }
    public Store Store { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Product p)
            return false;

        return Name == p.Name && Unit == p.Unit &&
               Store.Name == p.Store.Name &&
               ManufacturerId == p.ManufacturerId &&
               CategoryId == p.CategoryId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Unit, Store.Name, ManufacturerId, CategoryId);
    }
}