using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WatchAPI.DTOs.Watch;
using WatchAPI.Services;

namespace WatchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchesController : ControllerBase
    {
        private readonly IWatchService _service;

        public WatchesController(IWatchService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var watches = await _service.GetAllAsync();
            return Ok(watches);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var watch = await _service.GetByIdAsync(id);
            if (watch == null)
            {
                return NotFound();
            }
            return Ok(watch);
        }

        [HttpGet("check-name")]
        public async Task<IActionResult> GetByName(string name)
        {
            var watch = await _service.GetByNameAsync(name);
            if (watch == null)
            {
                return NotFound();
            }
            return Ok(watch);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdmin()
        {
            var watches = await _service.GetAllAdminAsync();
            return Ok(watches);
        }

        [HttpGet("admin/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminById(Guid id)
        {
            var watch = await _service.GetAdminByIdAsync(id);
            if (watch == null)
                return NotFound();
            return Ok(watch);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] WatchCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.ImageFile != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var filePath = Path.Combine("wwwroot/Images/Watches", fileName);
                using var stream = System.IO.File.Create(filePath);
                await dto.ImageFile.CopyToAsync(stream);
                dto.ImageUrl = $"Images/Watches/{fileName}";
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var watch = await _service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetAdminById), new { id = watch.Id }, watch);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromForm] WatchUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.ImageFile != null)
            {
                var currentWatch = await _service.GetAdminByIdAsync(id);
                if (currentWatch != null && !string.IsNullOrEmpty(currentWatch.ImageUrl))
                {
                    var oldPath = Path.Combine("wwwroot", currentWatch.ImageUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                var filePath = Path.Combine("wwwroot/Images/Watches", fileName);
                using var stream = System.IO.File.Create(filePath);
                await dto.ImageFile.CopyToAsync(stream);
                dto.ImageUrl = $"Images/Watches/{fileName}";
            }
            else
            {
                dto.ImageUrl = null;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updated = await _service.UpdateAsync(id, dto, userId);

            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var deleted = await _service.DeleteAsync(id, userId);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
