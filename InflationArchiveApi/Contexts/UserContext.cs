using InflationArchive.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Contexts;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserFavorites> UserFavorites { get; set; }

    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}