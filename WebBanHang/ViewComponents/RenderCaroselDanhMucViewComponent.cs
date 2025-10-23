using Microsoft.AspNetCore.Mvc;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Data;
using WebBanHang.DAL.Repository.UnitOfWork;
using WebBanHang.Models.Models;

namespace WebBanHang.ViewComponents
{
    public class RenderCaroselDanhMucViewComponent(ICategoryService _categoryService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Category> dm = await _categoryService.GetActiveCategories();
            return View("CaroselDanhMuc",dm);
        }
    }
}
