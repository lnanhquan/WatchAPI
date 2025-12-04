using WatchAPI.Models.Entities;
using WatchAPI.Repositories;

public interface IWatchRepository : IGenericRepository<Watch>
{
    Task<Watch?> GetByNameAsync(string name);
}
