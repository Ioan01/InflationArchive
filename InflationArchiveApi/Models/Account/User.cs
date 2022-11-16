using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Models.Account;

[Table("user")]
[Index(nameof(UserName),IsUnique=true)]
[Index(nameof(Email),IsUnique = true)]
public class User : IdentityUser<Guid>
{
    
}