using WatchAPI.Models.Base;

namespace WatchAPI.Models.Entities
{
    public class InvoiceDetail : BaseEntity
    {
        // Foreign key to Invoice
        public Guid InvoiceId { get; set; }

        // Navigation property: 1 detail belong to 1 invoice
        public Invoice Invoice { get; set; } = null!;

        // Foreign key to Watch
        public Guid WatchId { get; set; }

        // Navigation property: 1 detail belong to 1 watch
        public Watch Watch { get; set; } = null!;

        public int Quantity { get; set; }

        public int Price { get; set; }

        public int Total => Quantity * Price;
    }
}
