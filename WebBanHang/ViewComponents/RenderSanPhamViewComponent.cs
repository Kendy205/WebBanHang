using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Models;
namespace WebBanHang.ViewComponents
{
    public class RenderSanPhamViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Food f, int itemsPerRow = 4)
        {
            ViewBag.itemsPerRow = itemsPerRow;
            return View("RenderSanPham",f);
        }
    }
}
