using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims; // <— nhớ thêm
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

        // Helper: lấy userId chuẩn, nhanh
        private string? CurrentUserId => _userManager.GetUserId(User);
        // Hoặc nếu bạn thích async:
        // private async Task<string?> GetCurrentUserIdAsync() => (await _userManager.GetUserAsync(User))?.Id;

        // =============================
        // GET: Customer/Orders
        // =============================
        public async Task<IActionResult> Index(string status = "")
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // 1) Lỗi where trước đây là do thiếu await:
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            if (!string.IsNullOrWhiteSpace(status))
                orders = orders.Where(o => string.Equals(o.Status, status, StringComparison.OrdinalIgnoreCase));

            ViewBag.SelectedStatus = status;
            return View(orders.ToList());
        }

        // =============================
        // GET: Customer/Orders/Details/5
        // =============================
        public async Task<IActionResult> Details(int id)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // 2) Lỗi userId tại đây là do order là Task<Order> => cần await
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
        // GET: Customer/Orders/OrderSuccess/5
        // =============================
        [HttpGet]
        public async Task<IActionResult> OrderSuccess(int id)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || !string.Equals(order.UserId, userId, StringComparison.Ordinal))
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }
    }
}
