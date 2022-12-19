using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InflationArchive.Models.Requests;
using InflationArchive.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace InflationArchive.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TokenController : ControllerBase
{
    private readonly AccountService accountService;
    private readonly JwtSecurityTokenHandler tokenHandler = new();
    private readonly IConfiguration configuration;

    private readonly SecurityKey key;

    public TokenController([FromForm]AccountService accountService, IConfiguration configuration)
    {
        this.accountService = accountService;
        this.configuration = configuration;
        
        key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:IssuerSigningKey"])); 
    }
    
    [HttpPost]
    public async Task<IActionResult> Get([FromForm]UserLoginModel model)
    {

        if (!ModelState.IsValid) return BadRequest("Token failed to generate");

        var _user = await accountService.FindUserByUsernameOrEmail(model.LoginName);
        if (_user == null)
        {
            return Unauthorized();
        }

        var succeeded =  accountService.ValidateCredentials(_user, model.Password);
        if (!succeeded)
        {
            return Unauthorized();
        }


        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, _user.UserName),
            new Claim(JwtRegisteredClaimNames.NameId, _user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(configuration["JWT:Issuer"],
            configuration["JWT:Audience"],
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return Ok(tokenHandler.WriteToken(token));
    }
}