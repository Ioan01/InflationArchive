
using System.Web;
using InflationArchive.Helpers;
using InflationArchive.Models.Products;
using Newtonsoft.Json.Linq;


namespace InflationArchive.Services;

public class MetroScraper : AbstractStoreScraper
{
    // ids of the products
    private const string baseIdUrl = "https://produse.metro.ro/explore.articlesearch.v1/search?storeId=00013&language=ro-RO&country=RO&query=*&rows=1000&page=1&filter=%FILTER%";

    // data of the products queried
    private const string BaseDataUrl = "https://produse.metro.ro/evaluate.article.v1/betty-variants?storeIds=00013&country=RO&locale=ro-RO";


    public MetroScraper(HttpClient httpClient, ProductService productService) : base(httpClient, productService)
    {
    }

    protected override string StoreName => "Metro";

    private static List<string> GenerateDataUrls(List<JToken> idList)
    {
        var dataRequestUrls = new List<string>();
        var dataUrl = BaseDataUrl;


        for (var i = 1; i <= idList.Count; i++)
        {
            dataUrl += "&ids=" + idList[i - 1].Value<string>();
            if (i % 40 == 0)
            {
                dataRequestUrls.Add(dataUrl);

                dataUrl = BaseDataUrl;
            }
        }

        dataRequestUrls.Add(dataUrl);

        return dataRequestUrls;
    }

    private static double ExtractPrice(JToken item)
    {
        var priceInfo = item["stores"]!.First!.First!["sellingPriceInfo"]!;

        return priceInfo["finalPrice"]!.Value<double>();
    }

    private async Task<Product?> TryExtractProduct(JToken item, Category categoryRef)
    {
        item = item.First!;
        var outerData = item["variants"]!.First!.First!;


        var imageUrl = outerData["imageUrl"]!.Value<string>();
        var name = outerData["description"]!.Value<string>()!;


        var innerData = outerData["bundles"]!.First!.First!;

        var description = innerData["description"]!.Value<string>();
        if (description is null)
            return null;

        var manufacturerName = innerData["brandName"]!.Value<string>();
        if (manufacturerName is null)
            return null;

        var price = ExtractPrice(innerData);
        var qUnit = QuantityAndUnit.getPriceAndUnit(ref name);
        
        return new Product
        (
            description,
            imageUrl,
            Convert.ToDecimal(Math.Round(price / qUnit.Quantity, 2)),
            qUnit.Unit,
            categoryRef,
            await GetEntity<Manufacturer>(manufacturerName),
            await GetEntity<Store>(StoreName)
        );
    }
    
    protected override List<KeyValuePair<string, string[]>> GenerateRequests()
    {
        var requests = new List<KeyValuePair<string, string[]>>();
        
        
        foreach (var (category,categoryFilters) in Categories.MetroCategories)
        {
            requests.Add(new KeyValuePair<string, string[]>(category,
                categoryFilters.Select(str=> 
                    baseIdUrl.Replace("%FILTER%", 
                        HttpUtility.HtmlEncode($"category:alimentare/{str}"))).ToArray()));
        }
        
        
        
        return requests;
    }

    protected override async Task<IEnumerable<Product>> InterpretResponse(HttpResponseMessage responseMessage, Category categoryRef)
    {
        var products = new HashSet<Product>();


        var responseMessageContent = responseMessage.Content;
        var stringAsync = await responseMessageContent.ReadAsStringAsync();
        var result = JObject.Parse(stringAsync);

        var resultIds = result["resultIds"]!;


        var dataRequestUrls = GenerateDataUrls(resultIds.ToList());

        foreach (var url in dataRequestUrls)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Add("calltreeid", "a");

                var response = await HttpClient.SendAsync(requestMessage);

                var dataJson = JObject.Parse(await response.Content.ReadAsStringAsync());

                foreach (var item in dataJson["result"]!.Children().ToList())
                {
                    Product? product;
                    if ((product = await TryExtractProduct(item, categoryRef)) is not null)
                        products.Add(product);
                }
            }
            catch
            {
            }
        }

        return products;
    }
}