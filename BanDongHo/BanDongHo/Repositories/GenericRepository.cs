using Microsoft.EntityFrameworkCore;
using WatchAPI.Data;
using WatchAPI.Models.Base;

namespace WatchAPI.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : AuditableEntity
    {
        private readonly AppDbContext _db;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet
                         .Where(e => !e.IsDeleted)
                         .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                         .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<IEnumerable<T>> GetAllAdminAsync(bool? isDeleted = null)
        {
            var query = _dbSet.AsQueryable();

            if (isDeleted.HasValue)
                query = query.Where(e => e.IsDeleted == isDeleted.Value);

            return await query.ToListAsync();
        }


        public async Task<T?> GetAdminByIdAsync(Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateAsync(T entity, string? user)
        {
            entity.SetCreated(user);
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity, string? user)
        {
            entity.SetUpdated(user);
            _dbSet.Update(entity);
        }

        public async Task DeleteAsync(Guid id, string? user)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.SoftDelete(user);
            }
        }

        public async Task RestoreAsync(Guid id, string? user)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id);

            if (entity != null && entity.IsDeleted)
            {
                entity.Restore(user);
            }
        }
    }
}
