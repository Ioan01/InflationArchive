namespace InflationArchive.Models.Products;

public class Category : ScraperEntity
{
    public IEnumerable<Product> Products { get; set; }
}