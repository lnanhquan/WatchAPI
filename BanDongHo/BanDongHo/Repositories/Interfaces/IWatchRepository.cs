using WatchAPI.Models.Entities;
using WatchAPI.Repositories.Interfaces;

public interface IWatchRepository : IGenericRepository<Watch>
{
    Task<Watch?> GetByNameAsync(string name);
}
