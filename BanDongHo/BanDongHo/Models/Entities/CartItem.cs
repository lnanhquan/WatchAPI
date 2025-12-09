using WatchAPI.Models.Base;

namespace WatchAPI.Models.Entities
{
    public class CartItem : BaseEntity
    {
        // Foreign key to User
        public string UserId { get; set; } = string.Empty;

        // Navigation property: 1 cart item belong to 1 user
        public User User { get; set; } = null!;

        // Foreign key to Watch
        public Guid WatchId { get; set; }

        // Navigation property: 1 cart item belong to 1 watch
        public Watch Watch { get; set; } = null!;

        public int Quantity { get; set; }

        public int Total => Quantity * Watch.Price;
    }
}
