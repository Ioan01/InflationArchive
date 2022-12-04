using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using InflationArchive.Models.Products;
using Newtonsoft.Json.Linq;

namespace InflationArchive.Services;

public class MetroScraper : AbstractStoreScraper
{
    // ids of the products
    private static readonly string baseIdUrl =
        "https://produse.metro.ro/explore.articlesearch.v1/search?storeId=00013&language=ro-RO&country=RO&query=*&rows=1000&page=1&filter=%FILTER%";

    // data of the products queried
    private static readonly string baseDataUrl =
        "https://produse.metro.ro/evaluate.article.v1/betty-variants?storeIds=00013&country=RO&locale=ro-RO"; 
    
    
    public MetroScraper(HttpClient httpClient, ProductService productService) : base(httpClient, productService)
    {
    }

    protected override string StoreName => "Metro";

    private List<string> GenerateDataUrls(List<JToken> idList)
    {
        List<string> dataRequestUrls = new List<string>();
        string dataUrl = baseDataUrl;
        
        
        for (int i = 1; i <= idList.Count; i++)
        {
            dataUrl += "&ids=" + idList[i-1].Value<string>();
            if (i % 40 == 0)
            {
                dataRequestUrls.Add(dataUrl);

                dataUrl = baseDataUrl;
            }
        }

        dataRequestUrls.Add(dataUrl);

        return dataRequestUrls;
    }

    private double ExtractPrice(JToken item)
    {
        var priceInfo = item
            ["stores"].First.First
            ["sellingPriceInfo"];

        return priceInfo["finalPrice"].Value<double>();

    }

    private async Task<Product> ExtractProduct(JToken item,Category category)
    {
        item = item.First;
        var outerData = item["variants"].First.First;
        
        
        string imageUrl = outerData["imageUrl"].Value<string>();
        string name = outerData["description"]!.Value<string>();
        

        var innerData = outerData["bundles"].First.First;

        string description = innerData!["description"]!.Value<string>();
        string manufacturer = innerData["brandName"].Value<string>();
        double price = ExtractPrice(innerData);
        
        var qUnit = QuantityAndUnit.getPriceAndUnit(ref name);


        Manufacturer manufacturerRef = null;
        if (manufacturer != null)
           manufacturerRef  =  manufacturerReferences.ContainsKey(manufacturer)
                ? manufacturerReferences[manufacturer]
                : await CreateOrGetManufacturer(manufacturer);
        
        
        
        return new Product()
        {
            Name = description,
            Unit = qUnit.Unit,
            Manufacturer = manufacturerRef,
            Store = storeReference,
            Category = category,
            PricePerUnit = price / qUnit.Quantity,
            ImageUri = imageUrl
        };
    }

    protected override  List<KeyValuePair<string, string[]>> generateRequests()
    {
        var requests = new List<KeyValuePair<string, string[]>>();
        
        requests.Add(new KeyValuePair<string, string[]>("Fructe/Legume",new []
        {
            baseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/fructe-legume"))
            
        }));
        
        requests.Add(new KeyValuePair<string, string[]>("Carne",new []
        {
            baseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/carne")),
            baseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/peste"))
        }));
        
        requests.Add(new KeyValuePair<string, string[]>("Lactate/Oua",new []
        {
            baseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/lactate"))
        }));
        
        requests.Add(new KeyValuePair<string, string[]>("Mezeluri",new []
        {
            baseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/mezeluri"))
        }));
        
        
        
        return requests;
    }

    protected override async Task<IEnumerable<Product>> interpretResponse(HttpResponseMessage responseMessage, Category category)
    {
        List<Product> products = new List<Product>();


        var responseMessageContent = responseMessage.Content;
        var stringAsync = await responseMessageContent.ReadAsStringAsync();
        var result = JObject.Parse(stringAsync);

        var resultIds = result["resultIds"];
        
        
        var dataRequestUrls = GenerateDataUrls(resultIds.ToList());
        
        foreach (var url in dataRequestUrls)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("calltreeid","a");

            var response = await httpClient.SendAsync(requestMessage);

            var dataJson = JObject.Parse(await response.Content.ReadAsStringAsync());

            foreach (var item in dataJson["result"].Children().ToList())
            {
                products.Add(await ExtractProduct(item, category));
            }
        }
        
        
        
        return products;
    }

    
}