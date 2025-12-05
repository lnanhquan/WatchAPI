using AutoMapper;
using WatchAPI.DTOs.Watch;
using WatchAPI.Models.Entities;
using WatchAPI.Repositories;

namespace WatchAPI.Services
{
    public class WatchService : IWatchService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<WatchService> _logger;

        public WatchService(IUnitOfWork uow, IMapper mapper, ILogger<WatchService> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<WatchUserDTO>> GetAllAsync()
        {
            _logger.LogInformation("User requested all watches");
            var watches = await _uow.Watches.GetAllAsync();
            _logger.LogInformation("Returned {Count} watches", watches.Count());
            return _mapper.Map<IEnumerable<WatchUserDTO>>(watches);
        }

        public async Task<WatchUserDTO?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("User requested watch by ID: {WatchId}", id);
            var watch = await _uow.Watches.GetByIdAsync(id);
            if (watch == null)
            {
                _logger.LogWarning("Watch with ID {WatchId} not found", id);
                return null;
            }
            else
            {
                _logger.LogInformation("Returned watch {WatchName}", watch.Name);
                return _mapper.Map<WatchUserDTO?>(watch);
            } 
        }

        public async Task<Watch?> GetByNameAsync(string name)
        {
            _logger.LogInformation("User requested watch by Name: {WatchName}", name);
            var watch = await _uow.Watches.GetByNameAsync(name);
            if (watch == null)
            {
                _logger.LogWarning("Watch with name {WatchName} not found", name);
                return null;
            }
            else
            {
                _logger.LogInformation("Found watch {WatchId} with name {WatchName}", watch.Id, watch.Name);
                return watch;
            }
        }

        public async Task<IEnumerable<WatchAdminDTO>> GetAllAdminAsync(bool? isDeleted = null)
        {
            _logger.LogInformation("Admin requested all watches");
            var watches = await _uow.Watches.GetAllAdminAsync(isDeleted);
            _logger.LogInformation("Returned {Count} watches for admin", watches.Count());
            return _mapper.Map<IEnumerable<WatchAdminDTO>>(watches);
        }

        public async Task<WatchAdminDTO?> GetAdminByIdAsync(Guid id)
        {
            _logger.LogInformation("Admin requested watch by ID: {WatchId}", id);
            var watch = await _uow.Watches.GetAdminByIdAsync(id);
            if (watch == null)
            { 
                _logger.LogWarning("Watch with Id {WatchId} not found for admin", id);
                return null;
            }
            else
            {
                _logger.LogInformation("Returned watch {WatchName} for admin", watch.Name);
                return _mapper.Map<WatchAdminDTO?>(watch);
            }
        }

        public async Task<Watch> CreateAsync(WatchCreateDTO dto, string? user = null)
        {
            ValidateWatchCreate(dto);

            var exists = await _uow.Watches.GetByNameAsync(dto.Name);
            if (exists != null)
                throw new ArgumentException($"Watch name '{dto.Name}' already exists");

            var watch = _mapper.Map<Watch>(dto);
            watch.SetCreated(user);

            await _uow.Watches.CreateAsync(watch, user);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Created watch {Name} by user {User}", watch.Name, user);
            return watch;
        }

        public async Task<bool> UpdateAsync(Guid id, WatchUpdateDTO dto, string? user = null)
        {
            ValidateWatchUpdate(dto);

            var sameNameWatch = await _uow.Watches.GetByNameAsync(dto.Name);
            if (sameNameWatch != null && sameNameWatch.Id != id)
                throw new ArgumentException($"Another watch with name '{dto.Name}' already exists");

            var watch = await _uow.Watches.GetByIdAsync(id);
            if (watch == null)
                return false;

            _mapper.Map(dto, watch);
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                watch.ImageUrl = dto.ImageUrl;
            }
            watch.SetUpdated(user);

            _uow.Watches.Update(watch, user);
            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, string? user = null)
        {
            var watch = await _uow.Watches.GetByIdAsync(id);
            if (watch == null)
            {
                _logger.LogWarning("Watch with ID {WatchId} not found for deletion", id);
                return false;
            }

            await _uow.Watches.DeleteAsync(id, user);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Soft deleted watch {WatchId} by user {User}", id, user);

            return true;
        }

        public async Task<bool> RestoreAsync(Guid id, string? user = null)
        {
            var watch = await _uow.Watches.GetAdminByIdAsync(id);
            if (watch == null)
            {
                _logger.LogWarning("Watch with ID {WatchId} not found for restoration", id);
                return false;
            }
            else if (!watch.IsDeleted)
            {
                _logger.LogWarning("Watch with ID {WatchId} is not deleted, cannot restore", id);
                return false;
            }

            await _uow.Watches.RestoreAsync(id, user);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Restored watch {WatchId} by user {User}", id, user);

            return true;
        }

        private static void ValidateWatchCreate(WatchCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required");
            if (dto.Price <= 0)
                throw new ArgumentException("Price must be positive");
            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("Category is required");
            if (string.IsNullOrWhiteSpace(dto.Brand))
                throw new ArgumentException("Brand is required");
            if (string.IsNullOrWhiteSpace(dto.ImageUrl))
                throw new ArgumentException("Image is required for creation");
        }

        private static void ValidateWatchUpdate(WatchUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required");
            if (dto.Price <= 0)
                throw new ArgumentException("Price must be positive");
            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("Category is required");
            if (string.IsNullOrWhiteSpace(dto.Brand))
                throw new ArgumentException("Brand is required");
        }
    }
}
