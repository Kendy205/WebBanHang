using Microsoft.AspNetCore.Mvc;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Repository.UnitOfWork;
using WebBanHang.DTOs;
using WebBanHang.Models.Models;

namespace WebBanHang.ViewComponents
{
    public class RenderDanhMucViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;  
        public RenderDanhMucViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Category> categories =await _categoryService.GetActiveCategories();
            //List<DanhMucDTO> listDanhMucDTOs= new List<DanhMucDTO>();
            //foreach (var category in categories)
            //{
            //    listDanhMucDTOs.Add(new DanhMucDTO ( category.CategoryName ));
            //}
            //service xu ly load data
            return View("RenderDanhMucSanPham", categories);
        }
    }
}
