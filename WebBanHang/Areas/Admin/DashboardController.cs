using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.BLL.IServices;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IFoodService _foodService;
        private readonly ICategoryService _categoryService;

        public DashboardController(IOrderService orderService, IFoodService foodService, ICategoryService categoryService)
        {
            _orderService = orderService;
            _foodService = foodService;
            _categoryService = categoryService;
        }

        // ===============================
        // GET: Admin/Dashboard
        // ===============================
        public async Task<IActionResult> Index()
        {
            var allOrders = await _orderService.GetAllOrders();
            var allFoods = await _foodService.GetAllFoods();
            var allCategories = await _categoryService.GetAllCategories();

            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            // Đơn hàng hôm nay
            var todayOrders = allOrders.Where(o => o.OrderDate.Date == today);
            ViewBag.TodayOrdersCount = todayOrders.Count();
            ViewBag.TodayRevenue = todayOrders.Sum(o => o.TotalAmount);

            // Đơn hàng tháng này
            var monthOrders = allOrders.Where(o => o.OrderDate >= thisMonth);
            ViewBag.MonthOrdersCount = monthOrders.Count();
            ViewBag.MonthRevenue = monthOrders.Sum(o => o.TotalAmount);

            // Tổng số liệu
            ViewBag.TotalOrders = allOrders.Count();
            ViewBag.TotalRevenue = allOrders.Sum(o => o.TotalAmount);
            ViewBag.PendingOrders = allOrders.Count(o => o.Status == "Pending");
            ViewBag.TotalFoods = allFoods.Count();
            ViewBag.TotalCategories = allCategories.Count();

            // Đơn hàng gần đây
            ViewBag.RecentOrders = allOrders.OrderByDescending(o => o.OrderDate).Take(10);

            // Top món bán chạy
            ViewBag.TopFoods = await _foodService.GetTopRatedFoods(5);

            // Biểu đồ doanh thu 7 ngày gần đây
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-i))
                .Reverse()
                .ToList();

            var revenueData = last7Days.Select(date => new
            {
                Date = date.ToString("dd/MM"),
                Revenue = allOrders.Where(o => o.OrderDate.Date == date).Sum(o => o.TotalAmount)
            }).ToList();

            ViewBag.RevenueChartLabels = string.Join(",", revenueData.Select(r => $"'{r.Date}'"));
            ViewBag.RevenueChartData = string.Join(",", revenueData.Select(r => r.Revenue));

            return View(allFoods);
        }

        // ===============================
        // GET: Admin/Dashboard/Statistics (AJAX)
        // ===============================
        [HttpGet]
        public async Task<JsonResult> Statistics(string period = "week")
        {
            var allOrders = await _orderService.GetAllOrders();
            var today = DateTime.Today;
            DateTime startDate;

            switch (period.ToLower())
            {
                case "today":
                    startDate = today;
                    break;
                case "week":
                    startDate = today.AddDays(-7);
                    break;
                case "month":
                    startDate = today.AddMonths(-1);
                    break;
                case "year":
                    startDate = today.AddYears(-1);
                    break;
                default:
                    startDate = today.AddDays(-7);
                    break;
            }

            var orders = allOrders.Where(o => o.OrderDate >= startDate);

            return Json(new
            {
                totalOrders = orders.Count(),
                totalRevenue = orders.Sum(o => o.TotalAmount),
                completedOrders = orders.Count(o => o.Status == "Completed"),
                cancelledOrders = orders.Count(o => o.Status == "Cancelled")
            });
        }
    }
}
