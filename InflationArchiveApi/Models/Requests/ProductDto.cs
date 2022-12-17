using InflationArchive.Models.Products;

namespace InflationArchive.Models.Requests;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ImageUri { get; set; }
    public decimal PricePerUnit { get; set; }
    public string Unit { get; set; }
    public ICollection<ProductPriceDto> ProductPrices { get; set; }
    public string Category { get; set; }
    public string Manufacturer { get; set; }
    public string Store { get; set; }
}