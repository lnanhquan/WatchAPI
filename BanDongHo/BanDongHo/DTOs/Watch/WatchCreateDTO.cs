using System.ComponentModel.DataAnnotations;
using WatchAPI.Constants;

namespace WatchAPI.DTOs.Watch;

public class WatchCreateDTO
{
    [Required, MaxLength(WatchConstants.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(ValidationConstants.MinPrice, ValidationConstants.MaxPrice)]
    public int Price { get; set; }

    [Required, MaxLength(WatchConstants.MaxCategoryLength)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(WatchConstants.MaxBrandLength)]
    public string Brand { get; set; } = string.Empty;

    [MaxLength(WatchConstants.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    public IFormFile ImageFile { get; set; } = default!;

    [MaxLength(WatchConstants.MaxImageUrlLength)]
    public string? ImageUrl { get; set; }
}
