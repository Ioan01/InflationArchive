using System.Web;
using InflationArchive.Models.Products;

namespace InflationArchive.Services;

public class MetroScraper : AbstractStoreScraper
{
    // ids of the products
    private static readonly string idUrls =
        "https://produse.metro.ro/explore.articlesearch.v1/search?storeId=00013&language=ro-RO&country=RO&query=*&rows=1000&page=1&filter=%FILTER%";

    // data of the products queried
    private static readonly string dataUrls =
        "https://produse.metro.ro/evaluate.article.v1/betty-variants?storeIds=00013&country=RO&locale=ro-RO";    
    
    
    public MetroScraper(HttpClient httpClient, ProductService productService) : base(httpClient, productService)
    {
    }

    protected override string StoreName => "Metro";

    protected override  List<KeyValuePair<string, string[]>> generateRequests()
    {
        var requests = new List<KeyValuePair<string, string[]>>();
        
        requests.Add(new KeyValuePair<string, string[]>("Fructe/Legume",new []
        {
            idUrls.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/fructe-legume"))
        }));
        
        return requests;
    }

    protected override async Task<IEnumerable<Product>> interpretResponse(HttpResponseMessage responseMessage, Category category)
    {
        var responseMessageContent = responseMessage.Content;
        var stringAsync = await responseMessageContent.ReadAsStringAsync();


        return null;
    }

    
}