using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.ViewComponents
{
    public class RenderFeaturedProductViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("RenderFeaturedProduct");
        }
    }
}
