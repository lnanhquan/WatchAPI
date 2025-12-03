using BanDongHo.DTOs;

namespace WatchAPI.Services
{
    public interface ICartItemService
    {
        Task<List<CartItemDTO>> GetCartAsync(string userId);
        Task AddAsync(string userId, CartItemDTO dto);
        Task UpdateAsync(string userId, CartItemDTO dto);
        Task DeleteAsync(string userId, Guid watchId);
        Task ClearCartAsync(string userId);
        Task SaveChangesAsync();
    }
}
