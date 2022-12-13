using InflationArchive.Contexts;
using InflationArchive.Models.Account;
using InflationArchive.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Services;

public class AccountService
{
    private UserContext userContext;
    private PasswordHasher<User> passwordHasher;

    public AccountService(UserContext userContext)
    {
        this.userContext = userContext;
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
        return await userContext.Users.FirstOrDefaultAsync(user =>
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
}