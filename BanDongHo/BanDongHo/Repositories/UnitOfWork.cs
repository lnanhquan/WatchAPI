using WatchAPI.Data;
using WatchAPI.Repositories.Interfaces;

namespace WatchAPI.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public IWatchRepository Watches { get; }
    public ICartItemRepository CartItems { get; }
    public IInvoiceRepository Invoices { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;

        Watches = new WatchRepository(_db);
        CartItems = new CartItemRepository(_db);
        Invoices = new InvoiceRepository(_db);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
