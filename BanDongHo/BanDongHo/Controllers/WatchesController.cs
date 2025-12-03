using BanDongHo.DTOs;
using BanDongHo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            var dtos = watches.Select(w => new WatchDTO
            {
                Id = w.Id,
                Name = w.Name,
                Price = w.Price,
                Category = w.Category,
                Brand = w.Brand,
                Description = w.Description,
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
                Category = watch.Category,
                Brand = watch.Brand,
                Description = watch.Description,
                ImageUrl = watch.ImageUrl
            };
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] WatchDTO watchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var watch = await _service.CreateAsync(watchDto);

            var result = new WatchDTO
            {
                Id = watch.Id,
                Name = watch.Name,
                Price = watch.Price,
                Category = watch.Category,
                Brand = watch.Brand,
                Description = watch.Description,
                ImageUrl = watch.ImageUrl,
                ImageFile = null
            };

            return CreatedAtAction(nameof(GetById), new { id = watchDto.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromForm] WatchDTO watchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _service.UpdateAsync(id, watchDto);

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

        [HttpGet("check-name")]
        public async Task<IActionResult> GetByName(string name)
        {
            var watch = await _service.GetByNameAsync(name);
            if (watch == null)
            {
                return NotFound();
            }
            var dto = new WatchDTO
            {
                Id = watch.Id,
                Name = watch.Name,
                Price = watch.Price,
                Category = watch.Category,
                Brand = watch.Brand,
                Description = watch.Description,
                ImageUrl = watch.ImageUrl
            };
            return Ok(dto);
        }
    }
}
