using BanDongHo.DTOs;
using BanDongHo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WatchAPI.Models.Entities;

namespace BanDongHo.Services
{
    public class WatchService : IWatchService
    {
        private readonly IWatchRepository _repo;
        private readonly ILogger<WatchService> _logger;

        public WatchService(IWatchRepository repo, ILogger<WatchService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<Watch>> GetAllAsync()
        {
            try
            {
                var watches = await _repo.GetAllAsync();
                _logger.LogInformation("Retrieving all watches");
                return watches;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

        public async Task<Watch?> GetByIdAsync(Guid id)
        {
            try
            {
                var watch = await _repo.GetByIdAsync(id);

                if (watch == null)
                {
                    _logger.LogWarning("GetById: Watch with ID {Id} not found", id);
                }
                else
                {
                    _logger.LogInformation("GetById: Found watch {Name} with ID {Id}", watch.Name, watch.Id);
                }
                return watch;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

        public async Task<Watch> CreateAsync(WatchDTO dto)
        {
            try
            {
                var exists = await _repo.GetByNameAsync(dto.Name);
                if (exists != null)
                {
                    _logger.LogWarning("Create: Watch with name {Name} already exists", dto.Name);
                    throw new InvalidOperationException($"Watch with name '{dto.Name}' already exists.");
                }

                string imageUrl = null;

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Watches", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.ImageFile.CopyToAsync(stream);
                    }

                    imageUrl = $"/Images/Watches/{fileName}";
                }

                var watch = new Watch
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Category = dto.Category,
                    Brand = dto.Brand,
                    Description = dto.Description,
                    ImageUrl = imageUrl
                };

                await _repo.CreateAsync(watch);
                _logger.LogInformation("Created new watch {Name} with ID {Id}", watch.Name, watch.Id);
                return watch;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Guid id, WatchDTO dto)
        {
            try
            {
                var existingWatch = await _repo.GetByIdAsync(id);
                if (existingWatch == null)
                {
                    _logger.LogWarning("Update: Watch with ID {Id} not found", id);
                    return false;
                }
                var otherWatch = await _repo.GetByNameAsync(dto.Name);
                if (otherWatch != null && otherWatch.Id != id)
                {
                    _logger.LogWarning("Update: Watch with name {Name} already exists", dto.Name);
                    throw new InvalidOperationException($"Watch with name '{dto.Name}' already exists.");
                }

                string imageUrl = existingWatch.ImageUrl;

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingWatch.ImageUrl))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "Images", "Watches", Path.GetFileName(existingWatch.ImageUrl));
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Watches", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.ImageFile.CopyToAsync(stream);
                    }

                    imageUrl = $"/Images/Watches/{fileName}";
                }

                existingWatch.Name = dto.Name;
                existingWatch.Price = dto.Price;
                existingWatch.Category = dto.Category;
                existingWatch.Brand = dto.Brand;
                existingWatch.Description = dto.Description;
                existingWatch.ImageUrl = imageUrl;

                await _repo.UpdateAsync(existingWatch);
                _logger.LogInformation("Updated watch {Name} with ID {Id}", existingWatch.Name, existingWatch.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var existingWatch = await _repo.GetByIdAsync(id);
                if (existingWatch == null)
                {
                    _logger.LogWarning("Delete: Watch with ID {Id} not found", id);
                    return false;
                }

                if (!string.IsNullOrEmpty(existingWatch.ImageUrl))
                {
                    var fileName = Path.GetFileName(existingWatch.ImageUrl);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Watches", fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                            _logger.LogInformation("Deleted image file: {FilePath}", filePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete image file: {FilePath}", filePath);
                        }
                    }
                }

                await _repo.DeleteAsync(id);
                _logger.LogInformation("Deleted watch with ID {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

        public async Task<Watch?> GetByNameAsync(string name)
        {
            try
            {
                var watch = await _repo.GetByNameAsync(name);

                if (watch == null)
                {
                    _logger.LogWarning("GetById: Watch with name {Name} not found", name);
                }
                else
                {
                    _logger.LogInformation("GetById: Found watch with name {Name}", name);
                }
                return watch;
            }
            catch (Exception ex)
            {
                _logger.LogError("{ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

    }
}
