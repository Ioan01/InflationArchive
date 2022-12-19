using InflationArchive.Helpers;
using InflationArchive.Models.Products;
using InflationArchive.Models.Requests;
using InflationArchive.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InflationArchive.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly JointService _jointService;

    public ProductController(ProductService productService, JointService jointService)
    {
        _productService = productService;
        _jointService = jointService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] Filter filter)
    {
        var response = await _productService.GetProducts(filter);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<ProductDto>> GetProduct([FromQuery] Guid id)
    {
        var product = await _jointService.GetProduct(id);
        if (product is null)
            return NotFound();

        return Ok(ProductService.ProductToDto(product));
    }

    

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ProductQueryDto>> GetFavorites([FromQuery] Filter filter)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var userId = Guid.Parse(AccountService.GetUserId(HttpContext.User.Claims)!);

        var response = await _productService.GetFavoriteProducts(userId, filter);

        return Ok(response);
    }
}