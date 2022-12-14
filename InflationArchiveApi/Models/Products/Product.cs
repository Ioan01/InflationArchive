namespace InflationArchive.Models.Products;

public class Product : ScraperEntity
{
    public decimal PricePerUnit { get; set; }
    public string Unit { get; set; }
    public bool Unreliable { get; set; }

    public IEnumerable<ProductPrice> Prices;

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public int ManufacturerId { get; set; }
    public Manufacturer Manufacturer { get; set; }

    public int StoreId { get; set; }
    public Store Store { get; set; }
}