using System.ComponentModel.DataAnnotations.Schema;

namespace InflationArchive.Models.Products;

[Table("Manufacturer")]
public class Manufacturer : ScraperEntity
{
    public string? Website { get; set; }

    public ICollection<Product> Products { get; set; }
}