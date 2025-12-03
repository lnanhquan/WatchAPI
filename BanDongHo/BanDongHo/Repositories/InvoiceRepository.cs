using Microsoft.EntityFrameworkCore;
using WatchAPI.Data;
using WatchAPI.Models.Entities;

namespace BanDongHo.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _db;

        public InvoiceRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _db.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Watch)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
        {
            return await _db.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Watch)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            await _db.Invoices.AddAsync(invoice);
            await _db.SaveChangesAsync();
            return invoice;
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var invoice = await _db.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _db.Invoices.Remove(invoice);
                await _db.SaveChangesAsync();
            }
        }
    }
}
