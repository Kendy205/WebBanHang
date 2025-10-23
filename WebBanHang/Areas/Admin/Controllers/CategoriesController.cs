using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebBanHang.BLL.IServices;
using WebBanHang.FileUpload.IFileUpload;
using WebBanHang.Models.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;
        private readonly IBufferedFileUploadService _fileUploadService;

        public CategoriesController(
            ICategoryService categoryService,
            IBufferedFileUploadService fileUploadService,
            ILogger<CategoriesController> logger) : base(logger)
        {
            _categoryService = categoryService;
            _fileUploadService = fileUploadService;
        }

        // GET: /Admin/Categories
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategories();
            return View(categories);
        }

        // GET: /Admin/Categories/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int Categoriesid)
        {
            var category = await _categoryService.GetCategoryById(Categoriesid);
            if (category == null)
            {
                ShowError("Danh mục không tồn tại");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: /Admin/Categories/Create
        [HttpGet]
        public IActionResult Create() => View(new Category());

        // POST: /Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile? imageFile)
        {
            try
            {
                //if (!ModelState.IsValid)
                    //return View(category);

                // Upload hình ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    category.ImageUrl = await _fileUploadService.UploadFileAsync(imageFile);
                }

                await _categoryService.AddCategory(category);
                ShowSuccess("Thêm danh mục thành công");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                ShowError($"Lỗi: {ex.Message}");
                return View(category);
            }
        }

        // GET: /Admin/Categories/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int Categoriesid)
        {
            var category = await _categoryService.GetCategoryById(Categoriesid);
            if (category == null)
            {
                ShowError("Danh mục không tồn tại");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // POST: /Admin/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Categoriesid, Category category, IFormFile? imageFile)
        {
            try
            {
                if (Categoriesid != category.CategoryId)
                    return BadRequest();

                //if (!ModelState.IsValid)
                    //return View(category);

                var existing = await _categoryService.GetCategoryById(Categoriesid);
                if (existing == null)
                {
                    ShowError("Danh mục không tồn tại");
                    return RedirectToAction("Index");
                }

                // Upload hình ảnh mới
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Xóa ảnh cũ
                    //if (!string.IsNullOrEmpty(existing.ImageUrl))
                    //    await DeleteImageAsync(existing.ImageUrl);

                    existing.ImageUrl = await _fileUploadService.UploadFileAsync(imageFile);
                }
                else
                {
                    category.ImageUrl = existing.ImageUrl;
                }

                existing.CategoryName = category.CategoryName;
                existing.Description = category.Description;
                

                category.CreatedAt = existing.CreatedAt;
                await _categoryService.UpdateCategory(existing);
                ShowSuccess("Cập nhật danh mục thành công");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                ShowError($"Lỗi: {ex.Message}");
                return View(category);
            }
        }

        // GET: /Admin/Categories/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                ShowError("Danh mục không tồn tại");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // POST: /Admin/Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                if (category == null)
                {
                    ShowError("Danh mục không tồn tại");
                    return RedirectToAction("Index");
                }

                // Xóa hình ảnh
                //if (!string.IsNullOrEmpty(category.ImageUrl))
                //    await DeleteImageAsync(category.ImageUrl);

                await _categoryService.DeleteCategory(id);
                await _categoryService.DeleteCategory(id);
                ShowSuccess("Xóa danh mục thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                ShowError($"Không thể xóa: {ex.Message}");
            }

            return RedirectToAction("Index");
        }

        // Helper: Save image
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

        // Helper: Delete image
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