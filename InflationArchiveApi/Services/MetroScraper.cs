using InflationArchive.Models.Products;

namespace InflationArchive.Services;

public class MetroScraper : AbstractStoreScraper
{
    public MetroScraper(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override List<KeyValuePair<string, string[]>> generateRequests()
    {
        return null;
    }

    protected override IEnumerable<Product> interpretResponse(HttpResponseMessage responseMessage, string category)
    {
        return null;
    }
}