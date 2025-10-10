using Microsoft.AspNetCore.Mvc;
using WebBanHang.DTOs;

namespace WebBanHang.ViewComponents
{
    public class RenderDanhMucViewComponent : ViewComponent
    {
        List<DanhMucDTO> listItem = new List<DanhMucDTO>();
        public async Task<IViewComponentResult> InvokeAsync()
        {
            listItem.AddRange(new[] {
                new DanhMucDTO() { Name = "Trang chủ" },
                new DanhMucDTO() { Name = "Sản phẩm" },
                new DanhMucDTO() { Name = "Giới thiệu" },
                new DanhMucDTO() { Name = "Liên hệ" },
            });
            //service xu ly load data
            return View("RenderDanhMucSanPham",listItem);
        }
    }
}
