using InflationArchive.Models.Products;
using Microsoft.AspNetCore.Identity;

namespace InflationArchive.Models.Account;

public class User : IdentityUser<Guid>
{
    public ICollection<Product> FavoriteProducts { get; set; }
}