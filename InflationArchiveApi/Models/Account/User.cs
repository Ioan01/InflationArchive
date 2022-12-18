using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Models.Account;

[Index(nameof(UserName), nameof(Email), IsUnique = true)]
public class User : IdentityUser<Guid>
{
    
}