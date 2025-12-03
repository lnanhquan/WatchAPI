using BanDongHo.DTOs;

namespace WatchAPI.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDTO>> GetAllAsync();
        Task<InvoiceDTO?> GetByIdAsync(Guid id);
        Task<InvoiceDTO> CreateAsync(InvoiceDTO dto);
        Task<InvoiceDTO?> UpdateAsync(Guid id, InvoiceDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
