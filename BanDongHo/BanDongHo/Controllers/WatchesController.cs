using BanDongHo.DTOs;
using BanDongHo.Models;
using BanDongHo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BanDongHo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WatchesController : ControllerBase
    {
        private readonly IWatchService _service;
        private readonly ILogger<WatchesController> _logger;

        public WatchesController(IWatchService service, ILogger<WatchesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var watches = await _service.GetAllAsync();

            var dtos = watches.Select(w => new WatchDTO
            {
                Id = w.Id,
                Name = w.Name,
                Price = w.Price,
                ImageUrl = w.ImageUrl
            });

            return Ok(dtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var watch = await _service.GetByIdAsync(id);
            if (watch == null)
            {
                return NotFound();
            }
            var dto = new WatchDTO
            {
                Id = watch.Id,
                Name = watch.Name,
                Price = watch.Price,
                ImageUrl = watch.ImageUrl
            };
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] WatchDTO watchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var watch = new Watch
            {
                Name = watchDto.Name,
                Price = watchDto.Price,
                ImageUrl = watchDto.ImageUrl
            };
            await _service.CreateAsync(watch);
            
            var result = new WatchDTO
            {
                Id = watch.Id,
                Name = watch.Name,
                Price = watch.Price,
                ImageUrl = watch.ImageUrl
            };

            return CreatedAtAction(nameof(GetById), new { id = watch.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] WatchDTO watchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var watch = new Watch
            {
                Name = watchDto.Name,
                Price = watchDto.Price,
                ImageUrl = watchDto.ImageUrl
            };
            var updated = await _service.UpdateAsync(id, watch);
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
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
