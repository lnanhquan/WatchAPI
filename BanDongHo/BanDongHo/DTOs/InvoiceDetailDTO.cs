using System.ComponentModel.DataAnnotations;

namespace WatchAPI.DTOs
{
    public class InvoiceDetailDTO
    {
        public Guid Id { get; set; }

        [Required]
        public Guid InvoiceId { get; set; }

        [Required]
        public Guid WatchId { get; set; }

        public string WatchName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000.")]
        public int Quantity { get; set; }

        [Range(0, 1_000_000_000, ErrorMessage = "Price must be between 0 and 1,000,000,000.")]
        public int Price { get; set; }

        public int Total => Quantity * Price;
    }
}
