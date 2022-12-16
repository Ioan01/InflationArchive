namespace InflationArchive.Models.Products;

public class Manufacturer : ScraperEntity
{
    public string? Website { get; set; }

    public IEnumerable<Product> Products { get; set; }
}