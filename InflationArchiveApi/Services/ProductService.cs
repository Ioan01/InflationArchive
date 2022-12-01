using InflationArchive.Contexts;
using InflationArchive.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Services;

public class ProductService
{
    public ScraperContext scraperContext { get; }

    public ProductService(ScraperContext scraperContext)
    {
        this.scraperContext = scraperContext;
    }


    public async Task<T> GetEntityOrCreate<T>(DbSet<T> dbSet,string name) where T : ScraperEntity, new()
    {
        var entity = await dbSet.FirstOrDefaultAsync(obj=>obj.Name==name);
        if (entity == null)
        {
            entity = (await dbSet.AddAsync(new T { Name = name })).Entity;
            await scraperContext.SaveChangesAsync();
        }

        return entity;
    }

    public async Task AddPriceNode(Product product)
    {
        
    }
    
    public async Task SaveOrUpdateProducts(IEnumerable<Product> products)
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