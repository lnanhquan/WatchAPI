using BanDongHo.DTOs;
using BanDongHo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BanDongHo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _service;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _service = invoiceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetAll()
        {
            var invoices = await _service.GetAllAsync();
            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDTO>> GetById(Guid id)
        {
            var invoice = await _service.GetByIdAsync(id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceDTO>> Create([FromBody] InvoiceDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.Details == null || dto.Details.Count == 0)
            {
                return BadRequest("Invoice must have at least one watch.");
            }

            try
            {
                var invoice = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] InvoiceDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.Details == null || dto.Details.Count == 0)
            {
                return BadRequest("Invoice must have at least one watch.");
            }

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Invoice/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
