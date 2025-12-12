using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WatchAPI.DTOs;
using WatchAPI.Services.Interfaces;

namespace WatchAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoicesController(IInvoiceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var invoices = await _service.GetAllAsync();
        return Ok(invoices);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var invoice = await _service.GetByIdAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }
        return Ok(invoice);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDTO>> Create([FromBody] InvoiceDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var invoice = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }
}
