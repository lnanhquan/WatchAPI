using WatchAPI.DTOs.Watch;
using WatchAPI.Models.Entities;

namespace WatchAPI.Services
{
    public interface IWatchService
    {
        Task<IEnumerable<WatchUserDTO>> GetAllAsync();

        Task<WatchUserDTO?> GetByIdAsync(Guid id);

        Task<Watch?> GetByNameAsync(string name);

        Task<IEnumerable<WatchAdminDTO>> GetAllAdminAsync(bool? isDeleted = null);

        Task<WatchAdminDTO?> GetAdminByIdAsync(Guid id);

        Task<WatchAdminDTO> CreateAsync(WatchCreateDTO dto, string? user = null);

        Task<bool> UpdateAsync(Guid id, WatchUpdateDTO dto, string? user = null);

        Task<bool> DeleteAsync(Guid id, string? user = null);

        Task<bool> RestoreAsync(Guid id, string? user = null);
    }
}
