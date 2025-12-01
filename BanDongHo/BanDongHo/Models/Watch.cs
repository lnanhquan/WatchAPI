using System.ComponentModel.DataAnnotations;

namespace BanDongHo.Models
{
    public class Watch
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Watch name is required!")]
        [Display(Name = "Watch name")]
        [MaxLength(100, ErrorMessage = "Watch name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 1000000000, ErrorMessage = "Price must be between 0 and 1,000,000,000.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Category is required!")]
        [MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Brand is required!")]
        [MaxLength(50, ErrorMessage = "Brand cannot exceed 50 characters.")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Image is required!")]
        public string? ImageUrl { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
