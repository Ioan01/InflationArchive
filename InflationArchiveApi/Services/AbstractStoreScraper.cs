using InflationArchive.Models.Products;
using Quartz;

namespace InflationArchive.Services;

public abstract class AbstractStoreScraper : IJob
{
    private HttpClient httpClient;
    private List<KeyValuePair<string, string[]>> httpRequestsByCategory;
    private static int PRODUCT_CAPACITY = 200;

    protected AbstractStoreScraper(HttpClient httpClient)
    {
        this.httpClient = httpClient;

        httpRequestsByCategory = generateRequests();
    }

    protected abstract List<KeyValuePair<string,string[]>> generateRequests();

    protected abstract IEnumerable<Product> interpretResponse(HttpResponseMessage responseMessage,string category);
    
    
    

    public async Task fetchData()
    {
        foreach (var (category, requestMessages) in httpRequestsByCategory)
        {
            var categoryProducts = new List<Product>(PRODUCT_CAPACITY);
            
            foreach (var httpRequestMessage in requestMessages)
            {
                var response = await httpClient.GetAsync(httpRequestMessage);

                var products = interpretResponse(response, category);

                categoryProducts.AddRange(categoryProducts);
            }
        }
    }

    public async Task Execute(IJobExecutionContext context)
    {

        await Console.Out.WriteLineAsync("Greetings from HelloJob!");


        await Task.Delay(1000);
    }
}