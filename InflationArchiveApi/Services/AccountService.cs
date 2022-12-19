using System.Security.Claims;
using InflationArchive.Contexts;
using InflationArchive.Models.Account;
using InflationArchive.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Services;

public class AccountService
{
    private ScraperContext scraperContext;
    private PasswordHasher<User> passwordHasher;
    private readonly JointService _jointService;

    public AccountService(ScraperContext scraperContext, ProductService productService, JointService jointService)
    {
        this.scraperContext = scraperContext;
        passwordHasher = new PasswordHasher<User>();
        _jointService = jointService;
    }

    public async Task RegisterUser(UserRegisterModel model)
    {
        var user = await scraperContext.Users.AddAsync(new User()
        {
            Email = model.Email,
            UserName = model.Username,
        });
        user.Entity.PasswordHash = passwordHasher.HashPassword(user.Entity, model.Password);

        await scraperContext.SaveChangesAsync();
    }

    public async Task<User?> FindUserByUsernameOrEmail(string usernameOrEmail)
    {
        return await scraperContext.Users.SingleOrDefaultAsync(user =>
            user.Email == usernameOrEmail || user.UserName == usernameOrEmail);
    }

    public bool ValidateCredentials(User user, string password)
    {
        if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
        {
            return false;
        }

        return true;
    }

    public static Claim? GetUserIdClaim(IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(static claim => claim.Type == ClaimTypes.NameIdentifier);
    }

    public async Task<bool> AddFavoriteProduct(Guid userId, Guid productId)
    {
        var product = await _jointService.GetProduct(productId);

        if (product is null)
            return false;

        var user = await _jointService.GetUserById(userId);

        user.FavoriteProducts.Add(product);

        await scraperContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFavorite(Guid userId, Guid productId)
    {
        var product = await _jointService.GetProduct(productId);

        if (product is null)
            return false;

        var user = await _jointService.GetUserById(userId);

        user.FavoriteProducts.Remove(product);

        await scraperContext.SaveChangesAsync();

        return true;
    }
}