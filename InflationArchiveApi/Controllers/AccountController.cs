using System.Net;
using InflationArchive.Models.Account;
using InflationArchive.Services;
using Microsoft.AspNetCore.Mvc;

namespace InflationArchive.Controllers;
[ApiController]
[Route("[controller]/[action]")]

public class AccountController : ControllerBase
{
    private AccountService accountService;

    public AccountController([FromForm]AccountService accountService)
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
}