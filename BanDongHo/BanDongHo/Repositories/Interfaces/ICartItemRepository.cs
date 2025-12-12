using WatchAPI.Models.Entities;

namespace WatchAPI.Repositories.Interfaces;

public interface ICartItemRepository
{
    Task<List<CartItem>> GetCartAsync(string userId);
    Task<CartItem?> GetItemAsync(string userId, Guid watchId);
    Task CreateAsync(CartItem item);
    void Update(CartItem item);
    void Delete(CartItem item);
    Task ClearCartAsync(string userId);
}
