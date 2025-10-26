using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Data;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : BaseAdminController
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext _context;

        public OrdersController(
            IOrderService orderService,
            ApplicationDbContext context,
            ILogger<OrdersController> logger) : base(logger)
        {
            _orderService = orderService;
            _context = context;
        }

        // GET: /Admin/Orders
        [HttpGet]
        public async Task<IActionResult> Index(string? status = null, string? searchTerm = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Orders.Include(o => o.User).AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            // Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(o =>
                    o.OrderCode.Contains(searchTerm) ||
                    
                    o.PhoneNumber.Contains(searchTerm)
                );
            }

            var totalOrders = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SelectedStatus = status;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);
            ViewBag.CurrentPage = page;

            // Thống kê
            var allOrders = await _context.Orders.ToListAsync();
            ViewBag.PendingCount = allOrders.Count(o => o.Status == "Pending");
            ViewBag.ConfirmedCount = allOrders.Count(o => o.Status == "Confirmed");
            ViewBag.DeliveringCount = allOrders.Count(o => o.Status == "Delivering");
            ViewBag.CompletedCount = allOrders.Count(o => o.Status == "Completed");
            ViewBag.CancelledCount = allOrders.Count(o => o.Status == "Cancelled");

            return View(orders);
        }

        // GET: /Admin/Orders/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                ShowError("Đơn hàng không tồn tại");
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // POST: /Admin/Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    ShowError("Đơn hàng không tồn tại");
                    return RedirectToAction("Index");
                }

                order.Status = status;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                ShowSuccess($"Cập nhật trạng thái thành {status}");
                _logger.LogInformation($"Order {orderId} status updated to {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status");
                ShowError("Có lỗi xảy ra");
            }

            return RedirectToAction("Details", new { id = orderId });
        }

        // POST: /Admin/Orders/UpdateStatusAjax (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateStatusAjax(int orderId, string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return Json(new { success = false, message = "Đơn hàng không tồn tại" });

                order.Status = status;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status via AJAX");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Admin/Orders/Print/5
        [HttpGet]
        public async Task<IActionResult> Print(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                ShowError("Đơn hàng không tồn tại");
                return RedirectToAction("Index");
            }

            return View(order);
        }
    }
}
