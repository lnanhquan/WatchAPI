using AutoMapper;
using WatchAPI.DTOs;
using WatchAPI.Models.Entities;
using WatchAPI.Repositories;

namespace WatchAPI.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<CartItemService> _logger;

        public CartItemService(IUnitOfWork uow, IMapper mapper, ILogger<CartItemService> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CartItemDTO>> GetCartAsync(string userId)
        {
            var items = await _uow.CartItems.GetCartAsync(userId);

            _logger.LogInformation("Returned cart for user {UserId}", userId);

            return _mapper.Map<IEnumerable<CartItemDTO>>(items);
        }

        public async Task<CartItemDTO> CreateAsync(string userId, CartItemDTO dto)
        {
            ValidateCartItemCreate(userId, dto);

            var existing = await _uow.CartItems.GetItemAsync(userId, dto.WatchId);

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                _uow.CartItems.Update(existing);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Updated quantity of existing cart item. User {UserId}, watch {WatchId}", userId, dto.WatchId);
                return _mapper.Map<CartItemDTO>(existing);
            }

            var watch = await _uow.Watches.GetByIdAsync(dto.WatchId) ?? throw new InvalidOperationException($"Watch {dto.WatchId} not found.");
            var newItem = new CartItem
            {
                UserId = userId,
                WatchId = dto.WatchId,
                Quantity = dto.Quantity,
                Watch = watch
            };

            await _uow.CartItems.CreateAsync(newItem);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Added new cart item for user {UserId}, watch {WatchId}", userId, dto.WatchId);
            return _mapper.Map<CartItemDTO>(newItem);
        }

        public async Task<bool> UpdateAsync(string userId, CartItemDTO dto)
        {
            var existing = await _uow.CartItems.GetItemAsync(userId, dto.WatchId);
            if (existing == null)
            {
                _logger.LogWarning("Update failed. Cart item not found for user {UserId}, watch {WatchId}", userId, dto.WatchId);
                throw new InvalidOperationException("Cart item not found.");
            }

            if (dto.Quantity < 1 || dto.Quantity > 1000)
                throw new ArgumentException("Quantity must be between 1 and 1000");

            existing.Quantity = dto.Quantity;

            _uow.CartItems.Update(existing);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Updated cart item for user {UserId}, watch {WatchId}", userId, dto.WatchId);

            return true;
        }

        public async Task<bool> DeleteAsync(string userId, Guid watchId)
        {
            var existing = await _uow.CartItems.GetItemAsync(userId, watchId);
            if (existing == null)
            {
                _logger.LogWarning("Delete failed. Cart item not found for user {UserId}, watch {WatchId}", userId, watchId);
                return false;
            }

            _uow.CartItems.Delete(existing);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Deleted cart item for user {UserId}, watch {WatchId}", userId, watchId);

            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.");

            await _uow.CartItems.ClearCartAsync(userId);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Cleared cart for user {UserId}", userId);

            return true;
        }

        private static void ValidateCartItemCreate(string userId, CartItemDTO dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required");

            if (dto.WatchId == Guid.Empty)
                throw new ArgumentException("WatchId is required");

            if (dto.Quantity < 1 || dto.Quantity > 1000)
                throw new ArgumentException("Quantity must be between 1 and 1000");
        }
    }
}
