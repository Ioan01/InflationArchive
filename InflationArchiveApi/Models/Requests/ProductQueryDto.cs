namespace InflationArchive.Models.Requests;

public class ProductQueryDto
{
    public IEnumerable<ProductDto> Products { get; }
    public long TotalCount { get; }
    

    public ProductQueryDto(IEnumerable<ProductDto> products, long totalCount)
    {
        Products = products;
        TotalCount = totalCount;
    }
}