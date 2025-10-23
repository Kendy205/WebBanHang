using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanHang.BLL.IServices;
using WebBanHang.DTOs;

namespace WebBanHang.Controllers.Api
{
   
    [ApiController]
    [Authorize]
    [Route("api/cart")]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartApiController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Helper này không cần async vì nó chỉ đọc dữ liệu từ Claims
        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        /// <summary>
        /// POST: /api/cart/add - Thêm vào giỏ hàng (Bất đồng bộ)
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                await _cartService.AddToCart(userId, request.FoodId, request.Quantity);

                // Lấy thông tin cập nhật
                var itemCount = await _cartService.GetCartItemCount(userId);
                var cartTotal = await _cartService.GetCartTotal(userId);

                return Ok(new
                {
                    success = true,
                    message = "Đã thêm vào giỏ hàng",
                   data = new { cartItemCount = itemCount, cartTotal }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        /// <summary>
        /// PUT: /api/cart/update - Cập nhật số lượng (Bất đồng bộ)
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                await _cartService.UpdateCartItem(request.CartItemId, request.Quantity);

                var itemCount = await _cartService.GetCartItemCount(userId);
                var cartTotal = await _cartService.GetCartTotal(userId);

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật giỏ hàng thành công",
                    data = new { cartItemCount = itemCount, cartTotal }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        /// <summary>
        /// DELETE: /api/cart/remove/{cartItemId} - Xóa một mục (Bất đồng bộ)
        /// </summary>
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                await _cartService.RemoveFromCart(cartItemId);

                var itemCount = await _cartService.GetCartItemCount(userId);
                var cartTotal = await _cartService.GetCartTotal(userId);

                return Ok(new
                {
                    success = true,
                    message = "Đã xóa khỏi giỏ hàng",
                    data = new { cartItemCount = itemCount, cartTotal }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        /// <summary>
        /// DELETE: /api/cart/clear - Xóa tất cả giỏ hàng (Bất đồng bộ)
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                await _cartService.ClearCart(userId);

                return Ok(new { success = true, message = "Đã xóa tất cả giỏ hàng" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
        // =====================
        // Lấy tóm tắt giỏ hàng (dùng cho icon header)
        // =====================
        [HttpGet("summary")]
        public async Task<IActionResult> GetCartSummary()
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    // Nếu vì lý do nào đó không lấy được userId (dù đã qua [Authorize])
                    // trả về giỏ hàng trống.
                    return Ok(new { success = true, data = new { cartItemCount = 0, cartTotal = 0m } });
                }
                // tong so luong gia hang cua user
                var itemCount = await _cartService.GetCartItemCount(userId);
                //tong tien
                var cartTotal = await _cartService.GetCartTotal(userId);

                return Ok(new
                {
                    success = true,
                    data = new { cartItemCount=itemCount, cartTotal= cartTotal }
                });
            }
            catch (Exception ex)
            {
                // Trả về lỗi server nếu có vấn đề khi truy vấn
                return StatusCode(500, new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

        // Request models:
        // Thường nên để các lớp này trong thư mục /Models/ApiRequests hoặc /DTOs
       
    }
}
