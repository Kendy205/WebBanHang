using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.BLL.IServices;
using WebBanHang.FileUpload.IFileUpload;
using WebBanHang.Models.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FoodsController : BaseAdminController
    {
        private readonly IFoodService _foodService;
        private readonly ICategoryService _categoryService;
        private readonly IBufferedFileUploadService _fileUploadService;

        public FoodsController(
            IFoodService foodService,
            ICategoryService categoryService,
            IBufferedFileUploadService fileUploadService,
            ILogger<FoodsController> logger) : base(logger)
        {
            _foodService = foodService;
            _categoryService = categoryService;
            _fileUploadService = fileUploadService;
        }

        // GET: /Admin/Foods
        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId = null, string? searchTerm = null,string? sortBy = null, int page = 1, int pageSize = 10)
        {
            var (foods, totalRecords) = await _foodService.GetFoodsByFilter(
                categoryId,
                null,
                null,
                sortBy,
                page,
                pageSize
            );

            // Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var results = await _foodService.SearchFoods(searchTerm);

                if (categoryId != null)
                {
                    results = results.Where(f => f.CategoryId == categoryId).ToList();
                }

                foods = results;
                totalRecords = results.Count();
            }
            var categories = await _categoryService.GetAllCategories();

            
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", categoryId);
            ViewBag.CurrentCategory = categoryId;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SortBy = sortBy;

            return View(foods);
        }


        // GET: /Admin/Foods/Details/5
        //[HttpGet]
        //public IActionResult Details(int foodId)
        //{
        //    var food = _foodService.GetFoodById(foodId);
        //    if (food == null)
        //    {
        //        ShowError("Món ăn không tồn tại");
        //        return RedirectToAction("Index");
        //    }
        //    return View(food);
        //}

        // GET: /Admin/Foods/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(
                await _categoryService.GetActiveCategories(),
                "CategoryId",//gia tri that
                "CategoryName"//gia tri hien thi
            );
            return View();
        }

        // POST: /Admin/Foods/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Food food, IFormFile? imageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(
                       await _categoryService.GetActiveCategories(),
                        "CategoryId",
                        "CategoryName",
                        food.CategoryId
                    );
                    return View(food);
                }

                // Upload hình ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    food.ImageUrl = await _fileUploadService.UploadFileAsync(imageFile);
                }

                await _foodService.AddFood(food);
                ShowSuccess("Thêm món ăn thành công");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating food");
                ShowError($"Lỗi: {ex.Message}");

                ViewBag.Categories = new SelectList(
                    await _categoryService.GetActiveCategories(),
                    "CategoryId",
                    "CategoryName",
                    food.CategoryId
                );
                return View(food);
            }
        }

        // GET: /Admin/Foods/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int foodId)
        {
            var food = await _foodService.GetFoodById(foodId);
            if (food == null)
            {
                ShowError("Món ăn không tồn tại");
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(
                await _categoryService.GetActiveCategories(),
                "CategoryId",
                "CategoryName",
                food.CategoryId
            );
            return View(food);
        }

        // POST: /Admin/Foods/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int foodId, Food food, IFormFile? imageFile)
        {
            try
            {
                if (foodId != food.FoodId)
                    return BadRequest();

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(
                        await _categoryService.GetActiveCategories(),
                        "CategoryId",
                        "CategoryName",
                        food.CategoryId
                    );
                    return View(food);
                }

                var existing = await _foodService.GetFoodById(foodId);

                if (existing == null)
                {
                    ShowError("Món ăn không tồn tại");
                    return RedirectToAction("Index");
                }
                existing.FoodName = food.FoodName;
                existing.Description = food.Description;
                existing.Price = food.Price;
                existing.IsAvailable = food.IsAvailable;
                existing.CategoryId = food.CategoryId;
                existing.UpdatedAt = DateTime.Now;
                
                // Upload hình ảnh mới
                if (imageFile != null && imageFile.Length > 0)
                {
                    //if (!string.IsNullOrEmpty(existing.ImageUrl))
                    //    await DeleteImageAsync(existing.ImageUrl);

                    existing.ImageUrl = await _fileUploadService.UploadFileAsync(imageFile);
                }
                
                await _foodService.UpdateFood(existing);
                ShowSuccess("Cập nhật món ăn thành công");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating food");
                ShowError($"Lỗi: {ex.Message}");

                ViewBag.Categories = new SelectList(await _categoryService.GetActiveCategories(),"CategoryId","CategoryName",food.CategoryId);
                return View(food);
            }
        }

        public async Task<IActionResult> Delete(int foodId)
        {
            await _foodService.DeleteFood(foodId);
            return RedirectToAction("Index");
        }
        // POST: /Admin/Foods/ToggleAvailability (AJAX)
        //[HttpPost]
        //public async Task<IActionResult> ToggleAvailability(int id)
        //{
        //    try
        //    {
        //        var food = await _foodService.GetFoodById(id);
        //        if (food == null)
        //            return Json(new { success = false, message = "Món ăn không tồn tại" });

        //        food.IsAvailable = !food.IsAvailable;
        //        await _foodService.UpdateFood(food);

        //        return Json(new
        //        {
        //            success = true,
        //            isAvailable = food.IsAvailable,
        //            message = food.IsAvailable ? "Đã bật món ăn" : "Đã tắt món ăn"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error toggling food availability");
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        // Helper methods (same as CategoriesController)
        //private async Task<string> SaveImageAsync(IFormFile file, string folder)
        //{
        //    if (file == null || file.Length == 0)
        //        return null;

        //    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //    var extension = Path.GetExtension(file.FileName);
        //    fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

        //    var uploadPath = Path.Combine(_environment.WebRootPath, "FileUpload", folder);
        //    Directory.CreateDirectory(uploadPath);

        //    var fullPath = Path.Combine(uploadPath, fileName);
        //    using (var stream = new FileStream(fullPath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    return $"/FileUpload/{folder}/{fileName}";
        //}

        //private async Task DeleteImageAsync(string imageUrl)
        //{
        //    if (string.IsNullOrEmpty(imageUrl))
        //        return;

        //    var fullPath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));
        //    if (System.IO.File.Exists(fullPath))
        //    {
        //        System.IO.File.Delete(fullPath);
        //    }

        //    await Task.CompletedTask;
        //}
    }
}
