using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Data;
using WebBanHang.Models.Models;

namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IFoodService _foodService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, IFoodService foodService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _foodService = foodService;
            _userManager = userManager;
        }

        // =====================
        // Xem giỏ hàng
        // =====================
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            //if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var cart = await _cartService.GetCartByUserId(user.Id);
            HttpContext.Session.SetInt32("CartItemCount", cart?.TotalItems ?? 0);
            HttpContext.Session.SetString("CartTotal", (cart?.TotalAmount ?? 0m).ToString("F2"));

            return View(cart);
        }

        // =====================
        // Thêm món vào giỏ hàng
        // =====================
        [HttpPost]
        public async Task<JsonResult> AddToCart(int foodId, int quantity = 1)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, message = "Bạn cần đăng nhập!" });

                var food = await _foodService.GetFoodById(foodId);
                if (food == null || !food.IsAvailable)
                    return Json(new { success = false, message = "Món ăn không tồn tại hoặc đã ngừng bán." });

                await _cartService.AddToCart(user.Id, foodId, quantity);
                var cart = await _cartService.GetCartByUserId(user.Id);

                return Json(new
                {
                    success = true,
                    message = "Đã thêm vào giỏ hàng!",
                    totalItems = cart.TotalItems,
                    totalAmount = cart.TotalAmount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================
        // Cập nhật số lượng
        // =====================
        [HttpPost]
        [HttpPost]
        public async Task<JsonResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, message = "Chưa đăng nhập!" });

                // Cập nhật số lượng sản phẩm trong giỏ
                await _cartService.UpdateCartItem(cartItemId, quantity);

                // Lấy lại toàn bộ giỏ hàng sau khi cập nhật
                var cart = await _cartService.GetCartByUserId(user.Id);

                var updatedItem = cart.CartItems.FirstOrDefault(i => i.CartItemId == cartItemId);

                return Json(new
                {
                    success = true,
                    totalItems = cart.TotalItems,
                    cartItemCount = cart.TotalItems, // để frontend tương thích
                    totalAmount = cart.TotalAmount,
                    cartTotal = cart.TotalAmount,     // đồng bộ key
                    updatedItem = new
                    {
                        cartItemId = cartItemId,
                        subtotal = updatedItem?.Subtotal ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        // =====================
        // Xóa 1 món trong giỏ hàng
        // =====================
        [HttpPost]
        public async Task<JsonResult> RemoveItem(int cartItemId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, message = "Chưa đăng nhập!" });

                await _cartService.RemoveFromCart(cartItemId);
                var cart = await _cartService.GetCartByUserId(user.Id);

                return Json(new
                {
                    success = true,
                    message = "Đã xóa khỏi giỏ hàng",
                    totalItems = cart.TotalItems,
                    totalAmount = cart.TotalAmount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================
        // Xóa toàn bộ giỏ hàng
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                    await _cartService.ClearCart(user.Id);

                TempData["Success"] = "Đã xóa toàn bộ giỏ hàng!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // =====================
        // Lấy tóm tắt giỏ hàng (dùng cho icon header)
        // =====================
        [HttpGet]
        public async Task<JsonResult> GetCartSummary()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { totalItems = 0, totalAmount = 0m, items = new List<object>() });

            var cart = await _cartService.GetCartByUserId(user.Id);

            var items = cart.CartItems?.Select(i => new
            {
                i.CartItemId,
                FoodName = i.Food?.FoodName,
                i.Quantity,
                i.Price,
                i.Subtotal,
                i.Food?.ImageUrl
            }).ToList();

            return Json(new
            {
                totalItems = cart.TotalItems,
                totalAmount = cart.TotalAmount,
                items
            });
        }
        private async void SaveCartToCookie(string userId)
        {
            var cart = await _cartService.GetCartByUserId(userId);
            var cartData = new
            {
                ItemCount = cart.TotalItems,
                Total = cart.TotalAmount,
                UpdatedAt = DateTime.Now
            };

            var cookieValue = JsonConvert.SerializeObject(cartData);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("CartData", cookieValue, cookieOptions);
        }

    }
}
