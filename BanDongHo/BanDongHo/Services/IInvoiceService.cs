using BanDongHo.DTOs;
using BanDongHo.Models;

namespace BanDongHo.Services
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
