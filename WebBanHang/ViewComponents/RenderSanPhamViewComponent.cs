using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models.Models;
namespace WebBanHang.ViewComponents
{
    public class RenderSanPhamViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Food f)
        {
            return View("RenderSanPham",f);
        }
    }
}
