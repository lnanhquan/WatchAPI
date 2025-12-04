using System.ComponentModel.DataAnnotations;

namespace WatchAPI.DTOs.Watch
{
    public class WatchCreateDTO
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, Range(1, 1000000000)]
        public int Price { get; set; }

        [Required, MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Brand { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; } = default!;

        public string? ImageUrl { get; set; }
    }
}
