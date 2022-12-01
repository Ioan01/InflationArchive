using InflationArchive.Models.Products;

namespace InflationArchive.Services;

public class MetroScraper : AbstractStoreScraper
{
    public MetroScraper(HttpClient httpClient, ProductService productService) : base(httpClient, productService)
    {
    }

    protected override string StoreName => "Metro";

    protected override List<KeyValuePair<string, string[]>> generateRequests()
    {
        return null;
    }

    protected override IEnumerable<Product> interpretResponse(HttpResponseMessage responseMessage, Category category)
    {
        return null;
    }

    
}