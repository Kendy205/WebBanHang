using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
