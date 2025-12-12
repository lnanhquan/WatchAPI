using Microsoft.AspNetCore.Identity;

namespace WatchAPI.Models.Entities;

public class User : IdentityUser
{
    // Navigation property: 1 user can have multiple invoices
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    // Navigation property: 1 user can have multiple cart items
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }
}
