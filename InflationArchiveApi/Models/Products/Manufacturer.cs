namespace InflationArchive.Models.Products;

public class Manufacturer : ScraperEntity
{
    public string? Website { get; set; }

    public ICollection<Product> Products { get; set; }
}