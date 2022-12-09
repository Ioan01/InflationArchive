using System.Collections.Concurrent;
using InflationArchive.Models.Products;
using InflationArchive.Services.Helpers;
using Newtonsoft.Json.Linq;

namespace InflationArchive.Services;

public class MegaImageScraper : AbstractStoreScraper
{
    private const string RequestUrlBase =
        "https://api.mega-image.ro/?operationName=GetCategoryProductSearch&variables={\"lang\":\"ro\",\"category\":\"<insert_category>\",\"pageNumber\":<insert_page_number>,\"pageSize\":50}&extensions={\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"10ddc63b94cf5c83b7474746ae22bab24e83d503834a72942577672af7df4cb2\"}}";

    private readonly HttpClient _httpClient = new HttpClient();

    public MegaImageScraper(HttpClient httpClient, ProductService productService) : base(httpClient, productService)
    {
    }

    protected override string StoreName => "MegaImage";

    protected override List<KeyValuePair<string, string[]>> generateRequests()
    {
        var result = new ConcurrentBag<KeyValuePair<string, string[]>>();

        Parallel.ForEach(Categories.MegaImageCategories.Keys, key =>
        {
            result.Add(new KeyValuePair<string, string[]>
            (
                key, BuildRequestForCategory(key).Result.ToArray()
            ));
        });

        return result.ToList();
    }

    protected override Task<IEnumerable<Product>> interpretResponse(HttpResponseMessage responseMessage,
        Category category)
    {
        throw new NotImplementedException();
    }

    private async Task<IEnumerable<string>> BuildRequestForCategory(string category)
    {
        var result = new ConcurrentBag<string>();

        var codes = Categories.MegaImageCategories[category];

        await Parallel.ForEachAsync(codes, async (c, _) =>
        {
            var req = RequestUrlBase
                .Replace("<insert_category>", c)
                .Replace("<insert_page_number>", "0");

            var response = await _httpClient.GetAsync(req, _);
            var json = await response.Content.ReadAsStringAsync(_);

            var jObj = JObject.Parse(json);
            var nrPages = (int)jObj.SelectToken("data.categoryProductSearch.pagination.totalPages")!;

            for (var i = 0; i < nrPages; i++)
            {
                result.Add(RequestUrlBase
                    .Replace("<insert_category>", c)
                    .Replace("<insert_page_number>", i.ToString()));
            }
        });

        return result;
    }
}