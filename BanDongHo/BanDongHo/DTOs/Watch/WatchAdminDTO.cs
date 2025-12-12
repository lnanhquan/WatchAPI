namespace WatchAPI.DTOs.Watch;

public class WatchAdminDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
}
