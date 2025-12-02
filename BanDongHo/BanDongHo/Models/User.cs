using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BanDongHo.Models
{
    public class User : IdentityUser
    {
        // Navigation property: 1 User có nhiều hóa đơn
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        // Navigation property: 1 User có nhiều sản phẩm trong giỏ hàng
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
