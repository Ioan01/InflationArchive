using InflationArchive.Contexts;
using InflationArchive.Models.Products;

namespace InflationArchive.Services;

public class ProductService
{
    private ScraperContext scraperContext;

    public ProductService(ScraperContext scraperContext)
    {
        this.scraperContext = scraperContext;
    }
    

    public async Task SaveProducts(IEnumerable<Product> products, string category,string storeName)
    {
        await using (await scraperContext.Database.BeginTransactionAsync())
        {
            foreach (var product in products)
            {
                
            }

            await scraperContext.SaveChangesAsync();
        }
        
    }
    
}