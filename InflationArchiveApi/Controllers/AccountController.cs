using System.Net;
using InflationArchive.Models.Account;
using InflationArchive.Services;
using Microsoft.AspNetCore.Mvc;

namespace InflationArchive.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    private AccountService accountService;

    public AccountController([FromForm]AccountService accountService)
    {
        this.accountService = accountService;
    }

    [HttpPost("[controller]/login")]
    public async Task<ActionResult> Login([FromForm]UserLoginModel user)
    {
        if (!ModelState.IsValid)
            return Unauthorized();
        var _user = await accountService.FindUserByUsernameOrEmail(user.LoginName);
        if (_user == null)
        {
            return Unauthorized();
        }

        var succeeded =  accountService.ValidateCredentials(_user, user.Password);
        if (!succeeded)
        {
            return Unauthorized();
        }

        await accountService.SignInAsync(HttpContext, _user);

        return Ok();
    }
    
    [HttpPost("/register")]
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
}