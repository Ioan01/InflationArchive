using InflationArchive.Models.Products;
using Quartz;
using Quartz.Impl.Triggers;

namespace InflationArchive.Services;

public abstract class AbstractStoreScraper : IJob
{
    protected Store StoreReference = null!;
    protected static readonly Dictionary<string, Manufacturer> ManufacturerReferences = new();
    private static readonly Dictionary<string, Category> CategoryReferences = new();
    protected readonly HttpClient HttpClient;

    private readonly ProductService _productService;

    protected AbstractStoreScraper(HttpClient httpClient, ProductService productService)
    {
        HttpClient = httpClient;
        _productService = productService;
    }

    protected abstract string StoreName { get; }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await FetchData();
        }
        catch (Exception ex)
        {
            var retryTrigger = new SimpleTriggerImpl(Guid.NewGuid().ToString())
            {
                Description = "RetryTrigger",
                RepeatCount = 0,
                JobKey = context.JobDetail.Key,
                StartTimeUtc = DateBuilder.NextGivenMinuteDate(DateTime.Now, 30)
            };

            await context.Scheduler.ScheduleJob(retryTrigger);

            throw new JobExecutionException(ex, false);
        }
    }

    protected abstract List<KeyValuePair<string, string[]>> GenerateRequests();

    protected abstract Task<IEnumerable<Product>> InterpretResponse(HttpResponseMessage responseMessage,
        int categoryId);

    protected async Task<Manufacturer> CreateOrGetManufacturer(string manufacturerName)
    {
        var manufacturer = await _productService.GetEntityOrCreate(_productService.scraperContext.Manufacturers, manufacturerName);

        ManufacturerReferences.Add(manufacturerName, manufacturer);

        return manufacturer;
    }

    private async Task FetchData()
    {
        // if this is the first run when the app starts, load the store reference from the database, otherwise create it
        // after the first fetch, the store reference will be non-null
        StoreReference ??= await _productService.GetEntityOrCreate(_productService.scraperContext.Stores, StoreName);

        var httpRequestsByCategory = GenerateRequests();

        var categoryProducts = new HashSet<Product>();

        // for each category
        foreach (var (category, requestMessages) in httpRequestsByCategory)
        {
            // if this is the first run, the categoryReference dictionary will be empty, so we will populate it
            // after the first run of fetch, the dictionary will contain all key/value pairs
            if (!CategoryReferences.ContainsKey(category))
                CategoryReferences.Add(category,
                    await _productService.GetEntityOrCreate(_productService.scraperContext.Categories, category));

            // cave our category ref
            var categoryRef = CategoryReferences[category];

            // for each http request in the category
            foreach (var httpRequestMessage in requestMessages)
            {
                var response = await HttpClient.GetAsync(httpRequestMessage);

                // interpret the products in the request
                var responseProducts = await InterpretResponse(response, categoryRef.Id);

                // add the products from the request to a larger category list to bulk-update
                categoryProducts.UnionWith(responseProducts);
            }
        }

        // save or update every product
        await _productService.SaveOrUpdateProducts(categoryProducts);
    }
}