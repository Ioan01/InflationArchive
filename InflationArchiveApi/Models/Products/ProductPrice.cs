using System.ComponentModel.DataAnnotations.Schema;

namespace InflationArchive.Models.Products;

[Table("ProductPrice")]
public class ProductPrice
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; }
}