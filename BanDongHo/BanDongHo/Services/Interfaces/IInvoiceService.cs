using WatchAPI.DTOs;

namespace WatchAPI.Services.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDTO>> GetAllAsync();
    Task<InvoiceDTO?> GetByIdAsync(Guid id);
    Task<InvoiceDTO> CreateAsync(InvoiceDTO dto, string? user = null);
}
