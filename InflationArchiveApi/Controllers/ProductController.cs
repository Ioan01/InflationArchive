using InflationArchive.Helpers;
using InflationArchive.Models.Products;
using InflationArchive.Models.Requests;
using InflationArchive.Services;
using Microsoft.AspNetCore.Mvc;

namespace InflationArchive.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] Filter filter)
    {
        var products = await _productService.GetProducts(filter);
        return Ok(ProductsToDto(products));
    }

    [HttpGet]
    public async Task<ActionResult<ProductDto>> GetProduct([FromQuery] Guid id)
    {
        var product = await _productService.GetProduct(id);
        if (product is null)
            return NotFound();

        return Ok(ProductToDto(product));
    }

    private static IEnumerable<ProductDto> ProductsToDto(IEnumerable<Product> products)
    {
        return products.Select(static product => ProductToDto(product));
    }

    private static ProductDto ProductToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            ImageUri = product.ImageUri,
            PricePerUnit = product.PricePerUnit,
            Unit = product.Unit,
            ProductPrices = product.ProductPrices?.Select(static entry => new ProductPriceDto
            {
                Price = entry.Price,
                Date = entry.Date
            })?.ToList()!,
            Category = product.Category.Name,
            Manufacturer = product.Manufacturer.Name,
            Store = product.Store.Name,
        };
    }
}