namespace InflationArchive.Models.Products;

public class Product : ScraperEntity
{
    public Category Category { get; set; }
    
    public bool Unreliable { get; set; }
    
    public Manufacturer Manufacturer { get; set; }
    
    public Store Store { get; set; }
    
    public decimal PricePerUnit { get; set; }
    
    public string Unit { get; set; }

}