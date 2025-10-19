using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Data;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : BaseAdminController
    {
        private readonly IOrderService _orderService;
        private readonly IFoodService _foodService;
        private readonly ICategoryService _categoryService;
        private readonly ApplicationDbContext _context;

        public DashboardController(
            IOrderService orderService,
            IFoodService foodService,
            ICategoryService categoryService,
            ApplicationDbContext context,
            ILogger<DashboardController> logger) : base(logger)
        {
            _orderService = orderService;
            _foodService = foodService;
            _categoryService = categoryService;
            _context = context;
        }

        // GET: /Admin/Dashboard
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var allOrders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                    .ToListAsync();

                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                // Thống kê hôm nay
                var todayOrders = allOrders.Where(o => o.OrderDate.Date == today);
                ViewBag.TodayOrdersCount = todayOrders.Count();
                ViewBag.TodayRevenue = todayOrders.Sum(o => o.TotalAmount);

                // Thống kê tháng này
                var monthOrders = allOrders.Where(o => o.OrderDate >= thisMonth);
                ViewBag.MonthOrdersCount = monthOrders.Count();
                ViewBag.MonthRevenue = monthOrders.Sum(o => o.TotalAmount);

                // Tổng số liệu
                ViewBag.TotalOrders = allOrders.Count();
                ViewBag.TotalRevenue = allOrders.Sum(o => o.TotalAmount);
                ViewBag.PendingOrders = allOrders.Count(o => o.Status == "Pending");
                ViewBag.CompletedOrders = allOrders.Count(o => o.Status == "Completed");
                ViewBag.CancelledOrders = allOrders.Count(o => o.Status == "Cancelled");
                ViewBag.TotalFoods = await _context.Foods.CountAsync();
                ViewBag.TotalCategories = await _context.Categories.CountAsync();

                // Đơn hàng gần đây
                ViewBag.RecentOrders = allOrders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .ToList();

                // Biểu đồ doanh thu 7 ngày
                var last7Days = Enumerable.Range(0, 7)
                    .Select(i => today.AddDays(-i))
                    .Reverse()
                    .ToList();

                var revenueData = last7Days.Select(date => new
                {
                    Date = date.ToString("dd/MM"),
                    Revenue = allOrders.Where(o => o.OrderDate.Date == date).Sum(o => o.TotalAmount)
                }).ToList();

                ViewBag.RevenueChartLabels = string.Join(",", revenueData.Select(r => $"\"{r.Date}\""));
                ViewBag.RevenueChartData = string.Join(",", revenueData.Select(r => r.Revenue));

                _logger.LogInformation("Dashboard accessed");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                ShowError("Có lỗi xảy ra");
                return RedirectToAction("Index");
            }
        }

        // GET: /Admin/Dashboard/Statistics (AJAX)
        [HttpGet]
        public async Task<IActionResult> Statistics(string period = "week")
        {
            var allOrders = await _context.Orders.ToListAsync();
            var today = DateTime.Today;
            DateTime startDate = period switch
            {
                "today" => today,
                "week" => today.AddDays(-7),
                "month" => today.AddMonths(-1),
                "year" => today.AddYears(-1),
                _ => today.AddDays(-7)
            };

            var orders = allOrders.Where(o => o.OrderDate >= startDate).ToList();
            return Json(new
            {
                totalOrders = orders.Count(),
                totalRevenue = orders.Sum(o => o.TotalAmount),
                completedOrders = orders.Count(o => o.Status == "Completed"),
                cancelledOrders = orders.Count(o => o.Status == "Cancelled"),
                averageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0
            });
        }

    }
}
