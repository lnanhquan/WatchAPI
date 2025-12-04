namespace WatchAPI.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IWatchRepository Watches { get; }
        ICartItemRepository CartItems { get; }
        IInvoiceRepository Invoices { get; }

        Task<int> SaveChangesAsync();
    }
}
