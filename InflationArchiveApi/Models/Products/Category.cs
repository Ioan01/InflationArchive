namespace InflationArchive.Models.Products;

public class Category : ScraperEntity
{
    public ICollection<Product> Products { get; set; }
}