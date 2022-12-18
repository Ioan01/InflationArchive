using System.Security.Claims;
using InflationArchive.Helpers;
using InflationArchive.Models.Products;
using InflationArchive.Models.Requests;
using InflationArchive.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace InflationArchive.Controllers;
[ApiController]
[Route("[controller]/[action]")]

public class AccountController : ControllerBase
{
    private AccountService accountService;
    private ProductService productService;

    public AccountController(AccountService accountService, ProductService productService)
    {
        this.accountService = accountService;
        this.productService = productService;
    }
    
    
    [HttpPost]
    public async Task<ActionResult> Register([FromForm]UserRegisterModel user)
    {
        if (!ModelState.IsValid)
        {
            return Unauthorized();
        }

        if (user.Password != user.ConfirmPassword)
        {
            return BadRequest();
        }
        
        try
        {
            await accountService.RegisterUser(user);
            return Ok();
        }
        catch (Exception)
        {
            return Conflict();
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddFavorite([FromForm]Guid product )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userIdClaim = accountService.GetUserIdClaim(HttpContext.User.Claims);
        
        if (userIdClaim is null)
            return Unauthorized();
        
        var _product = await productService.GetProduct(product);
        if (_product is null)
            return NotFound();

        await accountService.AddFavoriteProduct(Guid.Parse(userIdClaim.Value), _product.Id);
        
        return Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveFavorite([FromForm] Guid product)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        

        var userIdClaim = accountService.GetUserIdClaim(HttpContext.User.Claims);
        
        if (userIdClaim is null)
            return Unauthorized();
        
        var _product = await productService.GetProduct(product);
        if (_product is null)
            return NotFound();

        await accountService.RemoveFavorite(Guid.Parse(userIdClaim.Value), _product.Id);

        return Ok();
    }

    
}