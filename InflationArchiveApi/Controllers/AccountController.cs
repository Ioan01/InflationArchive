using InflationArchive.Models.Requests;
using InflationArchive.Services;
using Microsoft.AspNetCore.Mvc;

namespace InflationArchive.Controllers;
[ApiController]
[Route("[controller]/[action]")]

public class AccountController : ControllerBase
{
    private AccountService accountService;

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

    // TODO: [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromForm] Guid productId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        // TODO: Get from token
        var userId = new Guid("039772cb-29c3-47ec-a46a-172e4b531d12");

        var ok = await accountService.AddFavoriteProduct(userId, productId);

        if (ok)
            return Ok();

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFavorite([FromForm] Guid productId)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        // TODO: Get from token
        var userId = new Guid("039772cb-29c3-47ec-a46a-172e4b531d12");

        var ok = await accountService.RemoveFavorite(userId, productId);

        if (ok)
            return Ok();

        return NotFound();
    }
}