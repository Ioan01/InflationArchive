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

    // NOTE: IQueryable is just an abstraction over something that will be turned into SQL
    //       by EF Core. It is NOT the result of the query. ToList() will actually return the result of the query.

    // Create filtered products query
    private static IQueryable<Product> FilterProducts(IQueryable<Product> products, Filter filter)
    {
        // Load (EF Core Include == SQL JOIN) related data
        var filtered = products
            .Include(static p => p.Category)
            .Include(static p => p.Manufacturer)
            .Include(static p => p.Store)
            // Generate filter query based on user input
            .Where(p =>
                EF.Functions.ILike(p.Name, $"%{filter.Name}%") &&
                EF.Functions.ILike(p.Category.Name, $"%{filter.Category}%") &&
                EF.Functions.ILike(p.Manufacturer.Name, $"%{filter.Manufacturer}%") &&
                p.PricePerUnit >= filter.MinPrice && p.PricePerUnit <= filter.MaxPrice);

        var descending = filter.Order == FilterConstants.Descending;
        var propertyName = filter.OrderBy switch
        {
            FilterConstants.OrderByPrice => nameof(Product.PricePerUnit),
            FilterConstants.OrderByName => nameof(Product.Name),
            _ => throw new InvalidEnumArgumentException()
        };

        // Generate order filtered query
        var ordered = filtered.OrderBy(propertyName, descending);

        return ordered;
    }

    public async Task<ProductQueryDto> GetProducts(Filter filter, Guid userId)
    {
        // Get user (with products not loaded!) if logged in
        var user = await scraperContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

        // Get filtered and ordered products QUERY (not yet executed!)
        var filtered = FilterProducts(scraperContext.Products, filter);

        // Get paginated products QUERY (not yet executed!)
        var productListQuery = filtered.Skip(filter.PageNr * filter.PageSize).Take(filter.PageSize);

        // Finally, execute products query and get result into memory
        var productList = await productListQuery.ToListAsync();

        // If user logged in
        if (user is not null)
            // Load into user only his favorites that match with queried products
            // Note the use of productListQuery instead of productList!
            await scraperContext.Entry(user)
                .Collection(static u => u.FavoriteProducts)
                .Query()
                .Where(fp => productListQuery.Any(pl => pl.Id == fp.Id))
                .LoadAsync();

        return new ProductQueryDto(ProductsToDto(productList, user), filtered.Count());
    }

    public async Task<ProductQueryDto> GetFavoriteProducts(Guid userId, Filter filter)
    {
        // Get user (with products not loaded!)
        var user = await scraperContext.Users.SingleAsync(u => u.Id == userId);

        // Create query to get user's favorites
        var products = scraperContext.Entry(user)
            .Collection(static u => u.FavoriteProducts)
            .Query();

        // Get filtered and ordered favorite products QUERY (not yet executed!)
        var filtered = FilterProducts(products, filter);

        // Execute paginated, filtered, favorite products query and get result into memory
        var productList = await filtered.Skip(filter.PageNr * filter.PageSize).Take(filter.PageSize).ToListAsync();

        return new ProductQueryDto(ProductsToDto(productList, user), filtered.Count());
    }

    public async Task<ProductDto?> GetProduct(Guid productId, Guid userId)
    {
        // Get user (with products not loaded!) if logged in
        var user = await scraperContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

        // Get product with related data loaded
        var product = await scraperContext.Products
            .Include(static p => p.Category)
            .Include(static p => p.Manufacturer)
            .Include(static p => p.Store)
            .Include(static p => p.ProductPrices)
            .SingleOrDefaultAsync(p => p.Id == productId);

        if (product is null) return null;

        // If user logged in
        if (user is not null)
            // Load into user only his favorites that match with queried product
            await scraperContext.Entry(user)
                .Collection(static u => u.FavoriteProducts)
                .Query()
                .Where(fp => product.Id == fp.Id)
                .LoadAsync();

        return ProductToDto(product, user);
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