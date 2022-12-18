using InflationArchive.Models.Products;

namespace InflationArchive.Models.Account;

public class UserFavorites
{
    public UserFavorites(Guid userId, Guid productId)
    {
        UserId = userId;
        ProductId = productId;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
}