using CyberHawksPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberHawksPortal.Controllers;

[Route("sponsors")]
public class SponsorsController : Controller
{
    private readonly ApplicationDbContext _db;
    public SponsorsController(ApplicationDbContext db) => _db = db;

    [Route("")]
    public IActionResult Index()
    {
        var sponsors = _db.Sponsors.AsNoTracking().OrderBy(s => s.SortOrder).ToList();
        return View(sponsors);
    }
}
