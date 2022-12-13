using InflationArchive.Contexts;
using InflationArchive.Models.Products;

namespace InflationArchive.Services;

public class ImageService
{
    private readonly ScraperContext context;
    private readonly HttpClient client;

    public ImageService(ScraperContext context, HttpClient client)
    {
        this.context = context;
        this.client = client;
    }

    public async Task<Image> UploadImage(string url)
    {
        await client.GetByteArrayAsync(url);
        return null;
    }
}