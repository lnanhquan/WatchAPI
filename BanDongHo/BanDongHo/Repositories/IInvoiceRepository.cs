using WatchAPI.Models.Entities;

namespace WatchAPI.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<IEnumerable<Invoice>> GetAllWithDetailAsync();
        Task<Invoice?> GetDetailAsync(Guid id);
    }
}
