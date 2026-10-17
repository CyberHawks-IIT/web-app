using Microsoft.AspNetCore.Mvc;

namespace CyberHawksPortal.Controllers;

[Route("competitions")]
public class CompetitionsController : Controller
{
    [Route("")]
    public IActionResult Index() => View();
}
