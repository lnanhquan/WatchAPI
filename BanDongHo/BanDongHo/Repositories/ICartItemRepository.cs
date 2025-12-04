using WatchAPI.Models.Entities;

namespace WatchAPI.Repositories
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetCartAsync(string userId);
        Task<CartItem?> GetItemAsync(string userId, Guid watchId);
        Task AddAsync(CartItem item);
        Task UpdateAsync(CartItem item);
        Task DeleteAsync(CartItem item);
        Task ClearCartAsync(string userId);
        Task SaveChangesAsync();
    }
}
