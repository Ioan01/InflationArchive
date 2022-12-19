using System.ComponentModel;
using InflationArchive.Contexts;
using InflationArchive.Helpers;
using InflationArchive.Models.Account;
using InflationArchive.Models.Products;
using InflationArchive.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Services;

public class ProductService
{
    private readonly ScraperContext scraperContext;

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

    public async Task AddPriceNode(Product product, DateTime dateTime)
    {
        var node = await scraperContext.ProductPrices
            .SingleOrDefaultAsync(n => n.ProductId == product.Id && n.Date == dateTime);

        if (node is not null)
            return;

        await scraperContext.ProductPrices.AddAsync(new ProductPrice
        {
            Price = product.PricePerUnit,
            Date = dateTime,
            ProductId = product.Id
        });
    }

    public async Task SaveOrUpdateProducts(IEnumerable<Product> products)
    {
        var dateTime = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour);

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
                productRef.ImageUri = product.ImageUri;
                scraperContext.Products.Update(productRef);
                await AddPriceNode(productRef, dateTime);
            }
            else
            {
                await scraperContext.Products.AddAsync(product);
                await AddPriceNode(product, dateTime);
            }
        }

        await scraperContext.SaveChangesAsync();
    }

    private static IQueryable<Product> FilterHelper(IQueryable<Product> filtered, Filter filter)
    {
        var descending = filter.Order == FilterConstants.Descending;
        var propertyName = filter.OrderBy switch
        {
            FilterConstants.OrderByPrice => nameof(Product.PricePerUnit),
            FilterConstants.OrderByName => nameof(Product.Name),
            _ => throw new InvalidEnumArgumentException()
        };

        var ordered = filtered.OrderBy(propertyName, descending);

        return ordered;
    }

    private static IQueryable<Product> FilterProducts(IEnumerable<Product> products, Filter filter)
    {
        var filtered = products.Where(p =>
            p.Name.Contains(filter.Name, StringComparison.InvariantCultureIgnoreCase) &&
            p.Category.Name.Contains(filter.Category, StringComparison.InvariantCultureIgnoreCase) &&
            p.Manufacturer.Name.Contains(filter.Manufacturer, StringComparison.InvariantCultureIgnoreCase) &&
            p.PricePerUnit >= filter.MinPrice && p.PricePerUnit <= filter.MaxPrice);
        return FilterHelper(filtered.AsQueryable(), filter);
    }

    private static IQueryable<Product> FilterProducts(IQueryable<Product> products, Filter filter)
    {
        var filtered = products.Where(p =>
            EF.Functions.ILike(p.Name, $"%{filter.Name}%") &&
            EF.Functions.ILike(p.Category.Name, $"%{filter.Category}%") &&
            EF.Functions.ILike(p.Manufacturer.Name, $"%{filter.Manufacturer}%") &&
            p.PricePerUnit >= filter.MinPrice && p.PricePerUnit <= filter.MaxPrice);
        return FilterHelper(filtered, filter);
    }

    public async Task<ProductQueryDto> GetProducts(Filter filter, Guid userId)
    {
        var user = await scraperContext.Users
            .Include(static u => u.FavoriteProducts)
            .SingleOrDefaultAsync(u => u.Id == userId);

        var products = scraperContext.Products
            .Include(static p => p.Category)
            .Include(static p => p.Manufacturer)
            .Include(static p => p.Store);

        var filtered = FilterProducts(products, filter);

        var productList = await filtered.Skip(filter.PageNr * filter.PageSize).Take(filter.PageSize).ToListAsync();

        return new ProductQueryDto(ProductsToDto(productList, user), filtered.Count());
    }

    public async Task<ProductQueryDto> GetFavoriteProducts(Guid userId, Filter filter)
    {
        var user = await scraperContext.Users
            .Include(static u => u.FavoriteProducts)
            .ThenInclude(static p => p.Category)
            .Include(static u => u.FavoriteProducts)
            .ThenInclude(static p => p.Manufacturer)
            .Include(static u => u.FavoriteProducts)
            .ThenInclude(static p => p.Store)
            .SingleAsync(u => u.Id == userId);

        var filtered = FilterProducts(user.FavoriteProducts, filter);

        var productList = filtered.Skip(filter.PageNr * filter.PageSize).Take(filter.PageSize);

        return new ProductQueryDto(ProductsToDto(productList, user), filtered.Count());
    }

    private static IEnumerable<ProductDto> ProductsToDto(IEnumerable<Product> products, User? user = null)
    {
        return products.Select(product => ProductToDto(product, user));
    }

    public static ProductDto ProductToDto(Product product, User? user = null)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            ImageUri = product.ImageUri,
            PricePerUnit = product.PricePerUnit,
            Unit = product.Unit,
            ProductPrices = product.ProductPrices?.Select(static entry => new ProductPriceDto
            {
                Price = entry.Price,
                Date = entry.Date
            })?.ToList()!,
            Category = product.Category.Name,
            Manufacturer = product.Manufacturer.Name,
            Store = product.Store.Name,
            IsFavoritedByCurrentUser = user?.FavoriteProducts.Any(p => p.Id == product.Id) ?? false
        };
    }
}