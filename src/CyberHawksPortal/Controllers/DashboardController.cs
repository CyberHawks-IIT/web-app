using CyberHawksPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberHawksPortal.Controllers;

[Route("dashboard")]
public class DashboardController : Controller
{
    [Route("")]
    public IActionResult Index()
    {
        var user = HttpContext.CurrentUser();
        if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = "/dashboard" });
        return View(user);
    }
}
