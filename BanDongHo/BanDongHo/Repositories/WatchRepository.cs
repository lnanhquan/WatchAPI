using Microsoft.EntityFrameworkCore;
using WatchAPI.Data;
using WatchAPI.Models.Entities;

namespace BanDongHo.Repositories
{
    public class WatchRepository : IWatchRepository
    {
        private readonly AppDbContext _db;
        public WatchRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(Watch watch)
        {
            //watch.Id = Guid.NewGuid(); 
            await _db.Watches.AddAsync(watch);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var watch = await _db.Watches.FindAsync(id);
            if (watch != null)
            {
                _db.Watches.Remove(watch);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Watch>> GetAllAsync()
        {
            return await _db.Watches.ToListAsync();
        }

        public async Task<Watch?> GetByIdAsync(Guid id)
        {
            return await _db.Watches.FindAsync(id);
        }

        public async Task UpdateAsync(Watch watch)
        {
            await _db.SaveChangesAsync();
        }

        public async Task<Watch?> GetByNameAsync(string name)
        {
            return await _db.Watches.FirstOrDefaultAsync(w => w.Name == name);
        }

    }
}
