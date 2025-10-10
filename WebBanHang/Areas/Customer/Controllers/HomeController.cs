using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WebBanHang.DAL.Data;

namespace WebBanHang.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        //private readonly Repository _db;
       
       
        public IActionResult Index()
        {
           // _db.Products.ToList();
            return View();
        }
    }
}
