using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Models.Account;

[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class User : IdentityUser<Guid>
{
    
}