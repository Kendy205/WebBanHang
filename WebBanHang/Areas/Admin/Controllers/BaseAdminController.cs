using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
   // [Route("Admin/{controller}/{action}/{id?}")]
    public class BaseAdminController : Controller
    {
        protected readonly ILogger<BaseAdminController> _logger;

        public BaseAdminController(ILogger<BaseAdminController> logger)
        {
            _logger = logger;
        }

        protected void ShowSuccess(string message) => TempData["Success"] = message;
        protected void ShowError(string message) => TempData["Error"] = message;
        protected void ShowWarning(string message) => TempData["Warning"] = message;

        protected string GetUserIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
