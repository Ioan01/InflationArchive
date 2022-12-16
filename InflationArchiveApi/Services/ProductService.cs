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

    public async Task<T> GetEntityOrCreate<T>(string name) where T : ScraperEntity, new()
    {
        var entity = await scraperContext.Set<T>().SingleOrDefaultAsync(obj => obj.Name == name);
        if (entity == null)
        {
            entity = (await scraperContext.Set<T>().AddAsync(new T { Name = name })).Entity;
            await scraperContext.SaveChangesAsync();
        }

        return entity;
    }

    public async Task AddPriceNode(Product product)
    {
        await scraperContext.ProductPrices.AddAsync(new ProductPrice
        {
            Price = product.PricePerUnit,
            Date = DateTime.UtcNow.Date,
            ProductId = product.Id
        });
    }

    public async Task SaveOrUpdateProducts(IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            var productRef = await scraperContext.Products
                .Include(static p => p.Category)
                .Include(static p => p.Manufacturer)
                .Include(static p => p.Store)
                .SingleOrDefaultAsync(p => product.Name == p.Name && product.Unit == p.Unit &&
                                           product.StoreName == p.Store.Name &&
                                           product.ManufacturerName == p.Manufacturer.Name &&
                                           product.CategoryName == p.Category.Name);

            if (productRef != null)
            {
                productRef.PricePerUnit = product.PricePerUnit;
                scraperContext.Products.Update(productRef);
                await AddPriceNode(productRef);
            }
            else
            {
                await scraperContext.Products.AddAsync(product);
                await AddPriceNode(product);
            }
        }

        await scraperContext.SaveChangesAsync();
    }
}