using InflationArchive.Models.Requests;
using InflationArchive.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InflationArchive.Controllers;
[ApiController]
[Route("[controller]/[action]")]

public class AccountController : ControllerBase
{
    private readonly AccountService accountService;

    public AccountController(AccountService accountService)
    {
        this.accountService = accountService;
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromForm] Guid productId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = Guid.Parse(AccountService.GetUserId(HttpContext.User.Claims)!);

        var ok = await accountService.AddFavoriteProduct(userId, productId);

        if (ok)
            return Ok();

        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveFavorite([FromForm] Guid productId)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var userId = Guid.Parse(AccountService.GetUserId(HttpContext.User.Claims)!);

        var ok = await accountService.RemoveFavorite(userId, productId);

        if (ok)
            return Ok();

        return NotFound();
    }
}