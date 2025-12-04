using System.ComponentModel.DataAnnotations;

namespace WatchAPI.DTOs
{
    public class InvoiceDTO
    {
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<InvoiceDetailDTO> Details { get; set; } = new List<InvoiceDetailDTO>();

        public int TotalAmount => Details.Sum(d => d.Total);
    }
}
