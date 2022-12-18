using System.ComponentModel.DataAnnotations.Schema;

namespace InflationArchive.Models.Products;

[Table("Category")]
public class Category : ScraperEntity
{
    public ICollection<Product> Products { get; set; }
}