using InflationArchive.Helpers;
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

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] Filter filter)
    {
        var idString = AccountService.GetUserId(HttpContext.User.Claims);
        var userId = idString is null ? Guid.Empty : Guid.Parse(idString);

        var response = await _productService.GetProducts(filter, userId);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<ProductDto>> GetProduct([FromQuery] Guid productId)
    {
        var idString = AccountService.GetUserId(HttpContext.User.Claims);
        var userId = idString is null ? Guid.Empty : Guid.Parse(idString);

        var product = await _productService.GetProduct(productId, userId);

        if (product is null)
            return NotFound();

        return Ok(product);
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