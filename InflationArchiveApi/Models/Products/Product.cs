namespace InflationArchive.Models.Products;

public class Product : ScraperEntity
{
    public Category Category { get; set; }
    
    public Manufacturer Manufacturer { get; set; }
    
    public Store? Store { get; set; }
    
    public double PricePerUnit { get; set; }
    
    public string Unit { get; set; }

    public string? ImageUri { get; set; }
    

}