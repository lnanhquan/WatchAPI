using System.ComponentModel.DataAnnotations;

namespace BanDongHo.DTOs
{
    public class WatchDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Watch name is required!")]
        [MaxLength(100, ErrorMessage = "Watch name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 1000000000, ErrorMessage = "Price must be between 0 and 1,000,000,000.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Category is required!")]
        [MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Brand is required!")]
        [MaxLength(50, ErrorMessage = "Brand cannot exceed 50 characters")]
        public string Brand { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
