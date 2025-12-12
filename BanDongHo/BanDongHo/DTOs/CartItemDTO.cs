using System.ComponentModel.DataAnnotations;
using WatchAPI.Constants;

namespace WatchAPI.DTOs;

public class CartItemDTO
{
    public Guid Id { get; set; }

    [Required]
    public Guid WatchId { get; set; }

    public string WatchName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    [Range(ValidationConstants.MinPrice, ValidationConstants.MaxPrice)]
    public int Price { get; set; }

    [Required]
    [Range(ValidationConstants.MinQuantity, ValidationConstants.MaxQuantity)]
    public int Quantity { get; set; }

    public int Total => Price * Quantity;
}
