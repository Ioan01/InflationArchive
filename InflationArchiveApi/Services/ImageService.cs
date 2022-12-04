using InflationArchive.Contexts;
using InflationArchive.Models.Products;

namespace InflationArchive.Services;

public class ImageService
{
    private ScraperContext context;

    public ImageService(ScraperContext context)
    {
        this.context = context;
    }

    public async Task<Image> UploadImage(string url)
    {
        return null;
    }
}