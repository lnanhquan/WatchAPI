using System.ComponentModel.DataAnnotations;

namespace WatchAPI.DTOs
{
    public class CartItemDTO
    {
        public Guid Id { get; set; }

        [Required]
        public Guid WatchId { get; set; }

        [Required]
        public string WatchName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        [Range(0, 1_000_000_000)]
        public int Price { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        public int Total => Price * Quantity;
    }
}
