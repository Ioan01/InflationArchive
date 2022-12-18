using System.ComponentModel.DataAnnotations.Schema;

namespace InflationArchive.Models.Products;

[Table("Store")]
public class Store : ScraperEntity
{
    public string? Website { get; set; }
    public ICollection<Product> Products { get; set; }
}