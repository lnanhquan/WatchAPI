using BanDongHo.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WatchAPI.Services;

namespace BanDongHo.Controllers
{
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
        public async Task<ActionResult<List<CartItemDTO>>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var cart = await _service.GetCartAsync(userId);
            return Ok(cart);
        }

        // POST: api/cartitems
        [HttpPost]
        public async Task<ActionResult> AddToCart([FromBody] CartItemDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            await _service.AddAsync(userId, dto);
            await _service.SaveChangesAsync();
            return Ok(new { message = "Item added to cart successfully." });
        }

        // PUT: api/cartitems
        [HttpPut]
        public async Task<ActionResult> UpdateCartItem([FromBody] CartItemDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            await _service.UpdateAsync(userId, dto);
            await _service.SaveChangesAsync();
            return Ok(new { message = "Cart item updated successfully." });
        }

        // DELETE: api/cartitems/{watchId}
        [HttpDelete("{watchId}")]
        public async Task<ActionResult> DeleteCartItem(Guid watchId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            await _service.DeleteAsync(userId, watchId);
            await _service.SaveChangesAsync();
            return Ok(new { message = "Cart item deleted successfully." });
        }

        // DELETE: api/cartitems/clear
        [HttpDelete("clear")]
        public async Task<ActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            await _service.ClearCartAsync(userId);
            await _service.SaveChangesAsync();
            return Ok(new { message = "Cart cleared successfully." });
        }
    }
}
