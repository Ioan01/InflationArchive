using System.Collections.Concurrent;
using InflationArchive.Models.Products;
using InflationArchive.Services.Helpers;
using Newtonsoft.Json.Linq;

namespace InflationArchive.Services;

public class MegaImageScraper : AbstractStoreScraper
{
    private const string RequestUrlBase =
        "https://api.mega-image.ro/?operationName=GetCategoryProductSearch&variables={\"lang\":\"ro\",\"category\":\"<insert_category>\",\"pageNumber\":<insert_page_number>,\"pageSize\":50}&extensions={\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"10ddc63b94cf5c83b7474746ae22bab24e83d503834a72942577672af7df4cb2\"}}";

    public MegaImageScraper(HttpClient httpClient, ProductService productService) : base(httpClient, productService)
    {
    }

    protected override string StoreName => "Mega Image";

    protected override List<KeyValuePair<string, string[]>> generateRequests()
    {
        var result = new ConcurrentBag<KeyValuePair<string, string[]>>();

        var tasks = Categories.MegaImageCategories.Keys.Select(async key =>
        {
            result.Add(new KeyValuePair<string, string[]>
            (
                key, (await BuildRequestForCategory(key)).ToArray()
            ));
        }).ToArray();

        Task.WaitAll(tasks);

        return result.ToList();
    }

    protected override async Task<IEnumerable<Product>> interpretResponse(HttpResponseMessage responseMessage,
        Category category)
    {
        var products = new List<Product>();

        var responseMessageContent = responseMessage.Content;
        var json = await responseMessageContent.ReadAsStringAsync();
        var result = JObject.Parse(json);

        var productTokens = result["data"]!["categoryProductSearch"]!["products"]!.ToList();

        foreach (var token in productTokens)
        {
            var manufacturerName = (string)token["manufacturerName"]!;
            var manufacturerRef = manufacturerReferences.ContainsKey(manufacturerName)
                ? manufacturerReferences[manufacturerName]
                : await CreateOrGetManufacturer(manufacturerName);

            products.Add(new Product
            {
                Name = (string)token["name"]!,
                Category = category,
                Manufacturer = manufacturerRef,
                Store = storeReference,
                PricePerUnit = (double)token.SelectToken("price.unitPrice")!,
                Unit = (string)token.SelectToken("price.unit")!,
                ImageUri = $"https://d1lqpgkqcok0l.cloudfront.net{(string)token["images"]!.Children().First()["url"]!}"
            });
        }

        return products;
    }

    private async Task<IEnumerable<string>> BuildRequestForCategory(string category)
    {
        var result = new ConcurrentBag<string>();

        var codes = Categories.MegaImageCategories[category];

        var tasks = codes.Select(async c =>
        {
            var req = RequestUrlBase
                .Replace("<insert_category>", c)
                .Replace("<insert_page_number>", "0");

            var response = await httpClient.GetAsync(req);
            var json = await response.Content.ReadAsStringAsync();

            var jObj = JObject.Parse(json);
            var nrPages = (int)jObj.SelectToken("data.categoryProductSearch.pagination.totalPages")!;

            for (var i = 0; i < nrPages; i++)
            {
                result.Add(RequestUrlBase
                    .Replace("<insert_category>", c)
                    .Replace("<insert_page_number>", i.ToString()));
            }
        });

        await Task.WhenAll(tasks);

        return result;
    }
}