using InflationArchive.Contexts;
using InflationArchive.Helpers;
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

    public async Task<IEnumerable<Product>> GetProducts(Filter filter)
    {
        var filtered = scraperContext.Products
            .Include(static p => p.Category)
            .Include(static p => p.Manufacturer)
            .Include(static p => p.Store)
            .Where(p =>
                EF.Functions.ILike(p.Name, $"%{filter.Name}%") &&
                EF.Functions.ILike(p.Category.Name, $"%{filter.Category}%") &&
                p.PricePerUnit >= filter.MinPrice && p.PricePerUnit <= filter.MaxPrice
            );

        IOrderedQueryable<Product> ordered;

        if (filter.Order == FilterConstants.Ascending)
        {
            ordered = filter.OrderBy == FilterConstants.OrderByPrice
                ? filtered.OrderBy(static p => p.PricePerUnit)
                : filtered.OrderBy(static p => p.Name);
        }

        else
        {
            ordered = filter.OrderBy == FilterConstants.OrderByPrice
                ? filtered.OrderByDescending(static p => p.PricePerUnit)
                : filtered.OrderByDescending(static p => p.Name);
        }

        return await ordered.Skip(filter.PageNr * filter.PageSize).Take(filter.PageSize).ToListAsync();
    }
}