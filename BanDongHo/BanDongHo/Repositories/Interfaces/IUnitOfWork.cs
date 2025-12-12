namespace WatchAPI.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IWatchRepository Watches { get; }
    ICartItemRepository CartItems { get; }
    IInvoiceRepository Invoices { get; }

    Task<int> SaveChangesAsync();
}
