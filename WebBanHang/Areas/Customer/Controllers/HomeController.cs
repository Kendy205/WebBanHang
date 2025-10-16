using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Data;

namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IFoodService _foodService;

        public HomeController(ICategoryService categoryService, IFoodService foodService)
        {
            _categoryService = categoryService;
            _foodService = foodService;
        }

        // ✅ Phải dùng async/await ở đây
        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu bất đồng bộ
            var categories = await _categoryService.GetActiveCategories();
            var topFoods = await _foodService.GetTopRatedFoods(8);
            var allFoods = await _foodService.GetAvailableFoods();

            // Truyền dữ liệu sang ViewBag + Model
            ViewBag.Categories = categories;
            ViewBag.TopRatedFoods = topFoods;

            return View(allFoods);
        }

        public IActionResult Blog()
        {
            return View();
        }

        

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string name, string email, string message)
        {
            TempData["Success"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất.";
            return RedirectToAction("Contact");
        }
        

    }
}
