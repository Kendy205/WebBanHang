using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Data;
using WebBanHang.Models.Models;

namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(
            IOrderService orderService,
            ICartService cartService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userManager = userManager;
        }

        private string? CurrentUserId => _userManager.GetUserId(User);

        // =============================
        // GET: Customer/Order (danh sách đơn hàng)
        // =============================
        public async Task<IActionResult> Index(string status = "")
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account", new { area = string.Empty });

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            if (!string.IsNullOrWhiteSpace(status))
                orders = orders.Where(o => string.Equals(o.Status, status, StringComparison.OrdinalIgnoreCase));

            ViewBag.SelectedStatus = status;
            return View(orders.ToList());
        }

        // =============================
        // GET: Customer/Order/Details/5
        // =============================
        public async Task<IActionResult> Details(int id)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = string.Empty });
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại.";
                return RedirectToAction(nameof(Index));
            }
            if (!string.Equals(order.UserId, userId, StringComparison.Ordinal))
            {
                TempData["Error"] = "Bạn không có quyền xem đơn hàng này.";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // =============================
        // GET: Customer/Order/Checkout (Trang thanh toán)
        // =============================
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account", new { area = string.Empty });

            var cart = await _cartService.GetCartByUserIdAsync(userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống. Vui lòng thêm món ăn trước khi đặt hàng.";
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Cart = cart; 
            return View();
        }

        // =============================
        // POST: Customer/Order/Checkout
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string shippingAddress, string phoneNumber,
            string paymentMethod, string? notes)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account", new { area = string.Empty });

            try
            {
                var cart = await _cartService.GetCartByUserIdAsync(userId);

                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    TempData["Error"] = "Giỏ hàng trống";
                    return RedirectToAction("Index", "Cart");
                }

                // Validate
                if (string.IsNullOrWhiteSpace(shippingAddress))
                    ModelState.AddModelError("shippingAddress", "Địa chỉ giao hàng không được để trống");

                if (string.IsNullOrWhiteSpace(phoneNumber))
                    ModelState.AddModelError("phoneNumber", "Số điện thoại không được để trống");

                if (string.IsNullOrWhiteSpace(paymentMethod))
                    ModelState.AddModelError("paymentMethod", "Vui lòng chọn phương thức thanh toán");

                if (!ModelState.IsValid)
                {
                    ViewBag.Cart = cart;
                    return View();
                }

                // Tạo đơn hàng
                var orderId = await _orderService.CreateOrderFromCartAsync(
                    userId,
                    shippingAddress,
                    phoneNumber,
                    paymentMethod,
                    notes ?? string.Empty
                );

                // Reset Session (ASP.NET Core)
                HttpContext.Session.SetInt32("CartItemCount", 0);
                HttpContext.Session.SetInt32("CartTotal", 0);

                TempData["Success"] = "Đặt hàng thành công! Cảm ơn bạn đã mua hàng.";

                return RedirectToAction(nameof(OrderSuccess), new { id = orderId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                ViewBag.Cart = cart;
                return View();
            }
        }

        // =============================
        // GET: Customer/Order/OrderSuccess/5
        // =============================
        [HttpGet]
        public async Task<IActionResult> OrderSuccess(int id)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = string.Empty });
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || !string.Equals(order.UserId, userId, StringComparison.Ordinal))
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // =============================
        // POST: Customer/Order/CancelOrder/5
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var userId = CurrentUserId;
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account", new { area = string.Empty });

                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null || !string.Equals(order.UserId, userId, StringComparison.Ordinal))
                {
                    TempData["Error"] = "Đơn hàng không tồn tại hoặc bạn không có quyền hủy";
                    return RedirectToAction(nameof(Index));
                }

                // Chỉ cho phép hủy đơn hàng Pending
                if (!string.Equals(order.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "Chỉ có thể hủy đơn hàng đang chờ xử lý";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await _orderService.CancelOrderAsync(id);
                TempData["Success"] = "Đã hủy đơn hàng thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // =============================
        // GET: Customer/Order/TrackOrder?orderCode=ORD20251010001
        // =============================
        [HttpGet]
        public async Task<IActionResult> TrackOrder(string? orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
            {
                TempData["Error"] = "Vui lòng nhập mã đơn hàng";
                return RedirectToAction(nameof(Index));
            }

            var order = await _orderService.GetOrderByCodeAsync(orderCode);
            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng với mã: " + orderCode;
                return RedirectToAction(nameof(Index));
            }

            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId) || !string.Equals(order.UserId, userId, StringComparison.Ordinal))
            {
                TempData["Error"] = "Bạn không có quyền xem đơn hàng này";
                return RedirectToAction(nameof(Index));
            }

            return View("Details", order);
        }
    }
}
