using InflationArchive.Helpers;
using InflationArchive.Models.Products;
using Quartz;
using Quartz.Impl.Triggers;

namespace InflationArchive.Services;

public abstract class AbstractStoreScraper : IJob
{
    private readonly ProductService _productService;
    private static readonly Dictionary<Type, Dictionary<string, ScraperEntity>> Caches = new();

    protected readonly HttpClient HttpClient;

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

    /*
     * Will return an IEnumerable<Product> that contains no duplicates
     */
    protected abstract Task<IEnumerable<Product>> InterpretResponse(HttpResponseMessage responseMessage,
        Category categoryRef);

    private static Dictionary<string, ScraperEntity> GetCacheFor<T>() where T : ScraperEntity
    {
        var key = typeof(T);

        Dictionary<string, ScraperEntity> cache;

        if (!Caches.ContainsKey(key))
        {
            cache = new();
            Caches[key] = cache;
        }
        else
        {
            cache = Caches[key];
        }

        return cache;
    }

    protected async Task<T> GetEntity<T>(string name) where T : ScraperEntity, new()
    {
        name = name.OnlyFirstCharToUpper();

        var cache = GetCacheFor<T>();

        T entity;

        if (!cache.ContainsKey(name))
        {
            entity = await _productService.GetEntityOrCreate<T>(name);
            cache[name] = entity;
        }
        else
        {
            entity = (T)cache[name];
        }

        return entity;
    }

    private async Task FetchData()
    {
        var dateTime = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour);

        var httpRequestsByCategory = GenerateRequests();

        // for each category
        foreach (var (categoryName, requestMessages) in httpRequestsByCategory)
        {
            var categoryRef = await GetEntity<Category>(categoryName);

            var categoryProducts = new HashSet<Product>();

            // for each http request in the category
            foreach (var httpRequestMessage in requestMessages)
            {
                var response = await HttpClient.GetAsync(httpRequestMessage);

                // interpret the products in the request
                var responseProducts = await InterpretResponse(response, categoryRef);

                // add the products from the request to a larger category list to bulk-update
                categoryProducts.UnionWith(responseProducts);
            }

            // save or update every product
            await _productService.SaveOrUpdateProducts(categoryProducts, dateTime);
        }
    }
}