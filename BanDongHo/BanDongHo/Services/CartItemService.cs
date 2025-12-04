using WatchAPI.DTOs;
using WatchAPI.Models.Entities;
using WatchAPI.Repositories;

namespace WatchAPI.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _repo;
        private readonly IWatchRepository _watchRepo;
        private readonly ILogger<CartItemService> _logger;

        public CartItemService(ICartItemRepository repo, IWatchRepository watchRepo, ILogger<CartItemService> logger)
        {
            _repo = repo;
            _watchRepo = watchRepo;
            _logger = logger;
        }

        public async Task<List<CartItemDTO>> GetCartAsync(string userId)
        {
            try
            {
                var items = await _repo.GetCartAsync(userId);

                var dtoList = items.Select(c => MapToDTO(c)).ToList();

                _logger.LogInformation("Retrieved cart for user {UserId}", userId);

                return dtoList;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving cart for user {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task AddAsync(string userId, CartItemDTO dto)
        {
            try
            {
                var existing = await _repo.GetItemAsync(userId, dto.WatchId);

                if (existing != null)
                {
                    existing.Quantity += dto.Quantity;
                    await _repo.UpdateAsync(existing);

                    _logger.LogInformation(
                        "Updated quantity of existing cart item. User {UserId}, watch {WatchId}",
                        userId, dto.WatchId);
                    return;
                }

                var watch = await _watchRepo.GetByIdAsync(dto.WatchId);
                if (watch == null)
                    throw new InvalidOperationException($"Watch {dto.WatchId} not found.");

                var newItem = new CartItem
                {
                    //Id = Guid.NewGuid(),
                    UserId = userId,
                    WatchId = dto.WatchId,
                    Quantity = dto.Quantity
                };

                await _repo.AddAsync(newItem);

                _logger.LogInformation(
                    "Added new cart item for user {UserId}, watch {WatchId}",
                    userId, dto.WatchId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding cart item for user {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task UpdateAsync(string userId, CartItemDTO dto)
        {
            try
            {
                var existing = await _repo.GetItemAsync(userId, dto.WatchId);
                if (existing == null)
                {
                    _logger.LogWarning("Update failed. Cart item not found for user {UserId}, watch {WatchId}",
                        userId, dto.WatchId);
                    throw new InvalidOperationException("Cart item not found.");
                }

                existing.Quantity = dto.Quantity;

                await _repo.UpdateAsync(existing);

                _logger.LogInformation("Updated cart item for user {UserId}, watch {WatchId}", userId, dto.WatchId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating cart item for user {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task DeleteAsync(string userId, Guid watchId)
        {
            try
            {
                var existing = await _repo.GetItemAsync(userId, watchId);
                if (existing == null)
                {
                    _logger.LogWarning("Delete failed. Cart item not found for user {UserId}, watch {WatchId}",
                        userId, watchId);
                    return;
                }

                await _repo.DeleteAsync(existing);

                _logger.LogInformation("Deleted cart item for user {UserId}, watch {WatchId}", userId, watchId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting cart item for user {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            try
            {
                await _repo.ClearCartAsync(userId);
                _logger.LogInformation("Cleared cart for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error clearing cart for user {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _repo.SaveChangesAsync();
                _logger.LogInformation("Saved changes for cart operations.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving changes: {Message}", ex.Message);
                throw;
            }
        }

        private CartItemDTO MapToDTO(CartItem c)
        {
            return new CartItemDTO
            {
                Id = c.Id,
                WatchId = c.WatchId,
                WatchName = c.Watch?.Name ?? string.Empty,
                ImageUrl = c.Watch?.ImageUrl,
                Price = c.Watch?.Price ?? 0,
                Quantity = c.Quantity
            };
        }
    }
}
