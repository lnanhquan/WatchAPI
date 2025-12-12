using WatchAPI.Models.Base;

namespace WatchAPI.Models.Entities;

public class Invoice : AuditableEntity
{
    // Foreign key to User
    public string UserId { get; set; } = string.Empty;

    // Navigation property: 1 invoice belong to 1 user
    public User User { get; set; } = null!;

    // Navigation property: 1 invoice has many invoice details
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public int TotalAmount => InvoiceDetails.Sum(detail => detail.Total);
}
