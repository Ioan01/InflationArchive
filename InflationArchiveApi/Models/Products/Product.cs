namespace InflationArchive.Models.Products;

public class Product : ScraperEntity
{
    public Category Category { get; set; }
    
    public Manufacturer Manufacturer { get; set; }
    
    public Store Store { get; set; }
    
    public double PricePerUnit { get; set; }
    
    public QuantityAndUnit QuantityAndUnit { get; set; }

}