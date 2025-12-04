using WatchAPI.Models.Base;

namespace WatchAPI.Repositories
{
    public interface IGenericRepository<T> where T : AuditableEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAdminAsync();
        Task<T?> GetAdminByIdAsync(Guid id);
        Task CreateAsync(T entity, string? user);
        void Update(T entity, string? user);
        Task DeleteAsync(Guid id, string? user);
    }
}
