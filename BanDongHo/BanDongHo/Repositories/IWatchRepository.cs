using WatchAPI.Models.Entities;

namespace BanDongHo.Repositories
{
    public interface IWatchRepository
    {
        Task<IEnumerable<Watch>> GetAllAsync();
        Task<Watch?> GetByIdAsync(Guid id);
        Task CreateAsync(Watch watch);
        Task UpdateAsync(Watch watch);
        Task DeleteAsync(Guid id);
        Task<Watch?> GetByNameAsync(string name);
    }
}
