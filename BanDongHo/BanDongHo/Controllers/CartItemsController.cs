using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WatchAPI.DTOs;
using WatchAPI.Services.Interfaces;

namespace WatchAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartItemsController : ControllerBase
{
    private readonly ICartItemService _service;

    public CartItemsController(ICartItemService service)
    {
        _service = service;
    }

    // GET: api/cartitems
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        var items = await _service.GetCartAsync(userId);
        return Ok(items);
    }

    // POST: api/cartitems
    [HttpPost]
    public async Task<ActionResult> AddToCart([FromBody] CartItemDTO dto)
    {
        var userId = GetUserId();
        await _service.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetCart), new { }, dto);
    }

    // PUT: api/cartitems
    [HttpPut]
    public async Task<ActionResult> UpdateCartItem([FromBody] CartItemDTO dto)
    {
        var userId = GetUserId();
        await _service.UpdateAsync(userId, dto);
        return NoContent();
    }

    // DELETE: api/cartitems/{watchId}
    [HttpDelete("{watchId}")]
    public async Task<ActionResult> DeleteCartItem(Guid watchId)
    {
        var userId = GetUserId();
        await _service.DeleteAsync(userId, watchId);
        return NoContent();
    }

    // DELETE: api/cartitems/clear
    [HttpDelete("clear")]
    public async Task<ActionResult> ClearCart()
    {
        var userId = GetUserId();
        await _service.ClearCartAsync(userId);
        return NoContent();
    }

    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            throw new UnauthorizedAccessException("User not authenticated.");

        }
        return userId;
    }

}
