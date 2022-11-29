using InflationArchive.Models.Products;

namespace InflationArchive.Services;

public abstract class AbstractStoreScraper
{
    private HttpClient httpClient;
    private List<KeyValuePair<string, string[]>> httpRequestsByCategory;

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
            foreach (var httpRequestMessage in requestMessages)
            {
                var response = await httpClient.GetAsync(httpRequestMessage);

                var products = interpretResponse(response, category);
            }
        }
    }
}