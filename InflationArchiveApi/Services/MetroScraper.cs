using System.Text.RegularExpressions;
using System.Web;
using InflationArchive.Models.Products;
using Newtonsoft.Json.Linq;

namespace InflationArchive.Services;

public class MetroScraper : AbstractStoreScraper
{
    private const string WeightsUnitRegex = @"\d+ *((mg)|g|(gr)|(kg)|(ml)|l)\b";

    // ids of the products
    private const string BaseIdUrl = "https://produse.metro.ro/explore.articlesearch.v1/search?storeId=00013&language=ro-RO&country=RO&query=*&rows=1000&page=1&filter=%FILTER%";

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


        return new Product
        (
            description,
            imageUrl,
            price,
            GetUnit(name),
            categoryRef,
            await GetEntity<Manufacturer>(manufacturerName),
            await GetEntity<Store>(StoreName)
        );
    }

    /*
     * From my observations, MegaImage only has two units, so maybe Metro should follow suit:
     *
     * 1) For "vrac" products, they use "Kg" because how else are you gonna measure them =))
     * 2) For any other type, like for example a 6x200g pack of cat food, they use "Piece"
     * because that pack is basically a piece.
     *
     * I have implemented exactly this:
     * 2) If there are weights followed by unit (eg: 200g) or "bucati" is explicitly stated,
     * then it means it's not "vrac" so use "Piece"
     * 1) Else, it's "vrac" so use "Kg"
     */
    private static string GetUnit(string name)
    {
        if (name.Contains("buc", StringComparison.InvariantCultureIgnoreCase) ||
            name.Contains("bucati", StringComparison.InvariantCultureIgnoreCase))
            return "Piece";

        return Regex.IsMatch(name, WeightsUnitRegex, RegexOptions.IgnoreCase)
            ? "Piece"
            : "Kg";
    }

    protected override List<KeyValuePair<string, string[]>> GenerateRequests()
    {
        var requests = new List<KeyValuePair<string, string[]>>
        {
            new("Fructe/Legume", new[]
            {
                BaseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/fructe-legume"))
            }),
            new("Carne", new[]
            {
                BaseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/carne")),
                BaseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/peste"))
            }),
            new("Lactate/Oua", new[]
            {
                BaseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/lactate"))
            }),
            new("Mezeluri", new[]
            {
                BaseIdUrl.Replace("%FILTER%",HttpUtility.UrlEncode("category:alimentare/mezeluri"))
            })
        };


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
                    else
                    {
                    }
                }
            }
            catch
            {
            }
        }

        return products;
    }
}