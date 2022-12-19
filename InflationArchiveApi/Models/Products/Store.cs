namespace InflationArchive.Models.Products;

public class Store : ScraperEntity
{
    public string? Website { get; set; }
    public ICollection<Product> Products { get; set; }
}