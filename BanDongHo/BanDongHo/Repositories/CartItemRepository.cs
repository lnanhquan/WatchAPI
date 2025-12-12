using Microsoft.EntityFrameworkCore;
using WatchAPI.Data;
using WatchAPI.Models.Entities;
using WatchAPI.Repositories.Interfaces;

namespace WatchAPI.Repositories;

public class CartItemRepository : ICartItemRepository
{
    private readonly AppDbContext _db;

    public CartItemRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CartItem>> GetCartAsync(string userId)
    {
        return await _db.CartItems
            .Include(c => c.Watch)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<CartItem?> GetItemAsync(string userId, Guid watchId)
    {
        return await _db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.WatchId == watchId);
    }

    public async Task CreateAsync(CartItem item)
    {
        await _db.CartItems.AddAsync(item);
    }

    public void Update(CartItem item)
    {
        _db.CartItems.Update(item);
    }

    public void Delete(CartItem item)
    {
        _db.CartItems.Remove(item);
    }

    public async Task ClearCartAsync(string userId)
    {
        var items = await _db.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        _db.CartItems.RemoveRange(items);
    }
}
