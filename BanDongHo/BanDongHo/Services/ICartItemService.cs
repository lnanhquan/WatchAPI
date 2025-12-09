using WatchAPI.DTOs;

namespace WatchAPI.Services
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItemDTO>> GetCartAsync(string userId);
        Task<CartItemDTO> CreateAsync(string userId, CartItemDTO dto);
        Task<bool> UpdateAsync(string userId, CartItemDTO dto);
        Task<bool> DeleteAsync(string userId, Guid watchId);
        Task<bool> ClearCartAsync(string userId);
    }
}
