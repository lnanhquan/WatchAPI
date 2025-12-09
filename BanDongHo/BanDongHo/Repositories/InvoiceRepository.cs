using Microsoft.EntityFrameworkCore;
using WatchAPI.Data;
using WatchAPI.Models.Entities;

namespace WatchAPI.Repositories
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly AppDbContext _db;

        public InvoiceRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Invoice>> GetAllWithDetailAsync()
        {
            return await _db.Invoices
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Include(x => x.User)
                .Include(x => x.InvoiceDetails)
                    .ThenInclude(d => d.Watch)
                .ToListAsync();
        }

        public async Task<Invoice?> GetDetailAsync(Guid id)
        {
            return await _db.Invoices
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.User)
                .Include(x => x.InvoiceDetails)
                    .ThenInclude(d => d.Watch)
                .FirstOrDefaultAsync();
        }
    }
}
