using System.ComponentModel.DataAnnotations;
using WatchAPI.Constants;

namespace WatchAPI.DTOs;

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
    [Range(ValidationConstants.MinQuantity, ValidationConstants.MaxQuantity)]
    public int Quantity { get; set; }

    [Range(ValidationConstants.MinPrice, ValidationConstants.MaxPrice)]
    public int Price { get; set; }

    public int Total => Quantity * Price;
}
