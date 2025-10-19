using Microsoft.AspNetCore.Mvc;
using WebBanHang.BLL.IServices;

namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class FoodController : Controller
    {
        private readonly IFoodService _foodService;
        private readonly ICategoryService _categoryService;

        public FoodController(IFoodService foodService, ICategoryService categoryService)
        {
            _foodService = foodService;
            _categoryService = categoryService;
        }
        public async Task<ActionResult> Index(int? categoryId, decimal? minPrice, decimal? maxPrice, string sortBy = "name", int page = 1, int pageSize = 12)
        {
            var (foods, totalRecords) = await _foodService.GetFoodsByFilter(
                categoryId,
                minPrice,
                maxPrice,
                sortBy,
                page,
                pageSize
            );

            ViewBag.Categories = await _categoryService.GetActiveCategories();
            ViewBag.CurrentCategory = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy;
            ViewBag.TopFood = await _foodService.GetTopRatedFoods(6);
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(foods);
        }

        // GET: Customer/Food/Details/5
        // Chi tiết món ăn
        public async Task<ActionResult> Details(int id)
        {
            var food = await _foodService.GetFoodById(id);

            if (food == null || !food.IsAvailable)
            {
                TempData["Error"] = "Món ăn không tồn tại hoặc không còn bán";
                return RedirectToAction("Index");
            }

            // Lấy món ăn liên quan (cùng danh mục)
            ViewBag.RelatedFoods = (await _foodService.GetFoodsByCategory(food.CategoryId))
                .Where(f => f.FoodId != id)
                .Take(4)
                .ToList();

            return View(food);
        }

        // GET: Customer/Food/Search
        // Tìm kiếm món ăn với AJAX
        public async Task<ActionResult> Search(string keyword, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 12)
        {
            
            var foods = await _foodService.SearchFoods(keyword);
            if (minPrice != null && maxPrice != null)
            {
                foods = foods.Where(f => f.Price >= minPrice && f.Price <= maxPrice).ToList();
            }

            var pagedFoods = foods
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Keyword = keyword;
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)foods.Count() / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Categories = await _categoryService.GetActiveCategories();
            ViewBag.Count = foods.Count();
            // 👉 Nếu là AJAX thì chỉ render phần danh sách món ăn
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_FoodListPartial", pagedFoods);
            }
            else
            {
                return View(pagedFoods);
            }
                
        }

        // GET: Customer/Food/Category/5
        // Món ăn theo danh mục
        public async Task<ActionResult> Category(int id, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 12)
        {
            var category = await _categoryService.GetCategoryById(id);
            var foods = await _foodService.GetFoodsByCategory(id);
            if (minPrice != null && maxPrice != null)
            {
                foods = foods.Where(f => f.Price >= minPrice && f.Price <= maxPrice).ToList();
            }
            foods = foods
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.CurrentCategory = id;
            ViewBag.Category = category;
            ViewBag.Categories = await _categoryService.GetActiveCategories();
            ViewBag.TotalPages = (int)System.Math.Ceiling(
                (double)(await _foodService.GetFoodsByCategory(id)).Count() / pageSize);
            ViewBag.CurrentPage = page;

            return View(foods);
        }

        // POST: Customer/Foods/QuickView (AJAX)
        // Xem nhanh món ăn
        //[HttpPost]
        //public JsonResult QuickView(int id)
        //{
        //    var food = await _foodService.GetFoodById(id);

        //    if (food == null || !food.IsAvailable)
        //    {
        //        return Json(new { success = false, message = "Món ăn không tồn tại" });
        //    }

        //    return Json(new
        //    {
        //        success = true,
        //        data = new
        //        {
        //            foodId = food.FoodId,
        //            foodName = food.FoodName,
        //            description = food.Description,
        //            price = food.Price,
        //            imageUrl = food.ImageUrl,
        //            rating = food.Rating,
        //            categoryName = food.Category?.CategoryName
        //        }
        //    });
        //}
    }
}
