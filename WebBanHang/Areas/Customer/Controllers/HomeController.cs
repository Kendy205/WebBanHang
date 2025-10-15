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

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _categoryService.GetActiveCategories();
            ViewBag.TopRatedFood =await _foodService.GetTopRatedFoods(8);
            var allFoods =await _foodService.GetAvailableFoods();
            return View(allFoods);
        }
    }
}
