using InflationArchive.Models.Products;

namespace InflationArchive.Services;

public class AuchanScraper : AbstractStoreScraper
{
    private const string RequestUrlBase = "";
    public AuchanScraper(HttpClient httpClient, ProductService productService) : base(httpClient, productService)
    {
    }

    protected override string StoreName => "Auchan";
    protected override List<KeyValuePair<string, string[]>> generateRequests()
    {
        throw new NotImplementedException();
    }

    protected override Task<IEnumerable<Product>> interpretResponse(HttpResponseMessage responseMessage, Category category)
    {
        throw new NotImplementedException();
    }
}