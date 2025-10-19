using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.DAL.Data;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    //[Route("Admin/{controller}/{action}/{id?}")]
    public class ReportsController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(
            ApplicationDbContext context,
            ILogger<ReportsController> logger) : base(logger)
        {
            _context = context;
        }

        // GET: /Admin/Reports
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Admin/Reports/Sales
        [HttpGet]
        public async Task<IActionResult> Sales(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Today.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Today;

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();

            // Doanh thu theo ngày
            var dailyRevenue = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToList();

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.TotalOrders = orders.Count();
            ViewBag.TotalRevenue = orders.Sum(o => o.TotalAmount);
            ViewBag.AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0;

            return View(dailyRevenue);
        }

        // GET: /Admin/Reports/TopFoods
        [HttpGet]
        public async Task<IActionResult> TopFoods(DateTime? startDate = null, DateTime? endDate = null, int top = 10)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Today.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Today;

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.OrderDetails)
                .ToListAsync();

            var topFoods = orders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => new { od.FoodId, od.FoodName })
                .Select(g => new
                {
                    FoodId = g.Key.FoodId,
                    FoodName = g.Key.FoodName,
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Subtotal),
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(top)
                .ToList();

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.Top = top;

            return View(topFoods);
        }

        // GET: /Admin/Reports/CustomerOrders
        [HttpGet]
        public async Task<IActionResult> CustomerOrders(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Today.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Today;

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.User)
                .ToListAsync();

            var customerStats = orders
                .GroupBy(o => new { o.UserId, o.User.Email })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    //FullName = g.Key.FullName,
                    Email = g.Key.Email,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    AverageOrderValue = g.Average(o => o.TotalAmount),
                    LastOrderDate = g.Max(o => o.OrderDate)
                })
                .OrderByDescending(x => x.TotalSpent)
                .ToList();

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            return View(customerStats);
        }

        // GET: /Admin/Reports/ExportSales (AJAX)
        [HttpGet]
        public async Task<IActionResult> ExportSales(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.User)
                .ToListAsync();

            var data = orders.Select(o => new
            {
                OrderCode = o.OrderCode,
                OrderDate = o.OrderDate.ToString("dd/MM/yyyy HH:mm"),
                //CustomerName = o.User.FullName,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod
            }).ToList();

            return Json(data);
        }
    }
}
