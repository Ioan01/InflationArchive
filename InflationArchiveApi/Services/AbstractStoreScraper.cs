using InflationArchive.Models.Products;
using Quartz;

namespace InflationArchive.Services;

public abstract class AbstractStoreScraper : IJob
{
    private static readonly Dictionary<string, Category> categoryReferences = new();

    protected static Dictionary<string, Manufacturer> manufacturerReferences = new();

    private static readonly int PRODUCT_CAPACITY = 200;
    protected HttpClient httpClient;
    private readonly List<KeyValuePair<string, string[]>> httpRequestsByCategory;

    private readonly ProductService productService;


    protected Store? storeReference;

    protected AbstractStoreScraper(HttpClient httpClient, ProductService productService)
    {
        this.httpClient = httpClient;
        this.productService = productService;

        httpRequestsByCategory = generateRequests();
    }


    protected abstract string StoreName { get; }

    public async Task Execute(IJobExecutionContext context)
    {
        await fetchData();
    }

    protected abstract List<KeyValuePair<string, string[]>> generateRequests();

    protected abstract Task<IEnumerable<Product>> interpretResponse(HttpResponseMessage responseMessage,
        Category category);

    protected async Task<Manufacturer> CreateOrGetManufacturer(string manufacturerName,
        string? manufacturerImage = null)
    {
        var manufacturer = await productService.GetEntityOrCreate(productService.scraperContext.Manufacturers, manufacturerName);
        
        manufacturerReferences.Add(manufacturerName,manufacturer);
        
        return manufacturer;
    }

    private async Task fetchData()
    {
        // if this is the first run when the app starts, load the store reference from the database, otherwise create it
        // aftet the first fetch, the store reference will be non-null
        storeReference ??= await productService.GetEntityOrCreate(productService.scraperContext.Stores, StoreName);

        // for each category
        foreach (var (category, requestMessages) in httpRequestsByCategory)
        {
            // if this is the first run, the categoryRefrence dictionary will be empty, so we will populate it 
            // after the first run of fetch, the dictionary will contain all key/value pairs 
            if (!categoryReferences.ContainsKey(category))
                categoryReferences.Add(category,
                    await productService.GetEntityOrCreate(productService.scraperContext.Categories, category));

            // cave our category ref
            var categoryRef = categoryReferences[category];


            var categoryProducts = new List<Product>(PRODUCT_CAPACITY);

            // for each http request in the category
            foreach (var httpRequestMessage in requestMessages)
            {
                var response = await httpClient.GetAsync(httpRequestMessage);

                // interpret the products in the reuqest
                var responseProducts = await interpretResponse(response, categoryRef);

                // add the products from the request to a larger category list to bulk-update
                categoryProducts.AddRange(responseProducts);
            }

            // save or update every product
            await productService.SaveOrUpdateProducts(categoryProducts);
        }
    }
}