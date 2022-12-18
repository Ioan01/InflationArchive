using System.Security.Claims;
using InflationArchive.Contexts;
using InflationArchive.Models.Account;
using InflationArchive.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Services;

public class AccountService
{
    private UserContext userContext;
    private ScraperContext scraperContext;
    private PasswordHasher<User> passwordHasher;

    public AccountService(UserContext userContext, ScraperContext scraperContext)
    {
        this.userContext = userContext;
        this.scraperContext = scraperContext;
        passwordHasher = new PasswordHasher<User>();
    }

    public async Task RegisterUser(UserRegisterModel model)
    {
        var user = await userContext.Users.AddAsync(new User()
        {
            Email = model.Email,
            UserName = model.Username,
        });
        user.Entity.PasswordHash = passwordHasher.HashPassword(user.Entity, model.Password);

        await userContext.SaveChangesAsync();
    }

    public async Task<User?> FindUserByUsernameOrEmail(string usernameOrEmail)
    {
        return await userContext.Users.SingleOrDefaultAsync(user =>
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

    public async Task<User?> FindUserById(Guid idGuid)
    {
        return await userContext.Users.FirstOrDefaultAsync(u => u.Id == idGuid);
    }

    

    public Claim? GetUserIdClaim(IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
    }

    public async Task AddFavoriteProduct(Guid userId, Guid productId)
    {
        var fav = await userContext.UserFavorites.FirstOrDefaultAsync(f =>
            f.UserId == userId && f.ProductId == productId);
        if (fav is null)
        {
            await userContext.UserFavorites.AddAsync(new UserFavorites(userId, productId));
        }

        await userContext.SaveChangesAsync();
    }

    public async Task RemoveFavorite(Guid userId, Guid productId)
    {
        var fav = await userContext.UserFavorites.FirstOrDefaultAsync(f =>
            f.UserId == userId && f.ProductId == productId);
        if (fav is not null)
        {
            userContext.UserFavorites.Remove(fav);
        }

        await userContext.SaveChangesAsync();
    }
}