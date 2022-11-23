using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InflationArchive.Controllers;

[ApiController]
[Authorize]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public string GetName()
    {
        return HttpContext.User.Identity.Name;
    }
}