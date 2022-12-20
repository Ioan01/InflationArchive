using System.Security.Claims;
using InflationArchive.Contexts;
using InflationArchive.Models.Account;
using InflationArchive.Models.Products;
using InflationArchive.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Services;

public class AccountService
{
    private ScraperContext scraperContext;
    private PasswordHasher<User> passwordHasher;

    public AccountService(ScraperContext scraperContext)
    {
        this.scraperContext = scraperContext;
        passwordHasher = new PasswordHasher<User>();
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

    public static string? GetUserId(IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(static claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task<User> GetUserById(Guid userId)
    {
        return await scraperContext.Users
            .Include(static u => u.FavoriteProducts)
            .SingleAsync(u => u.Id == userId);
    }

    public async Task<Product?> GetLazyProduct(Guid productId)
    {
        return await scraperContext.Products
            .SingleOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<bool> AddFavoriteProduct(Guid userId, Guid productId)
    {
        var product = await GetLazyProduct(productId);

        if (product is null)
            return false;

        var user = await GetUserById(userId);

        user.FavoriteProducts.Add(product);

        await scraperContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFavorite(Guid userId, Guid productId)
    {
        var product = await GetLazyProduct(productId);

        if (product is null)
            return false;

        var user = await GetUserById(userId);

        user.FavoriteProducts.Remove(product);

        await scraperContext.SaveChangesAsync();

        return true;
    }
}