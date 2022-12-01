using InflationArchive.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Contexts;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
        this.Database.EnsureCreated();
    }
}