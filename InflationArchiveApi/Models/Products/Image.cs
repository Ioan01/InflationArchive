namespace InflationArchive.Models.Products;

public class Image
{
    public int Id { get; set; }
    public byte[]? Data { get; set; }
    public string? Url { get; set; }
}