using BanDongHo.DTOs;
using WatchAPI.Models.Entities;

namespace BanDongHo.Services
{
    public interface IWatchService
    {
        Task<IEnumerable<Watch>> GetAllAsync();
        Task<Watch?> GetByIdAsync(Guid id);
        Task<Watch> CreateAsync(WatchDTO dto);
        Task<bool> UpdateAsync(Guid id, WatchDTO dto);
        Task<bool> DeleteAsync(Guid id);

        Task<Watch?> GetByNameAsync(string name);
    }
}
