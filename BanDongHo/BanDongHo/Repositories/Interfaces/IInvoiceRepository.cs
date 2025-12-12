using WatchAPI.Models.Entities;

namespace WatchAPI.Repositories.Interfaces;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<IEnumerable<Invoice>> GetAllWithDetailAsync();
    Task<Invoice?> GetDetailAsync(Guid id);
}
