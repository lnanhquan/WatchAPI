using WatchAPI.Models.Base;

namespace WatchAPI.Models.Entities;

public class Watch : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
}
