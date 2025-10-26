// Controllers/ErrorController.cs
using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    [Route("Error/Handle")]
    public IActionResult HandleError(int code)
    {
        if (code == 404)
            return View("Error404");

        //if (code == 500)
        //    return View("Error500");

        return View("Error");
    }
    [Route("Error/AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }
}