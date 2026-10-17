using CyberHawksPortal.Data;
using CyberHawksPortal.Models;
using Microsoft.AspNetCore.Mvc;

namespace CyberHawksPortal.Controllers;

[Route("search")]
public class SearchController : Controller
{
    private readonly ApplicationDbContext _db;
    public SearchController(ApplicationDbContext db) => _db = db;

    [Route("")]
    public IActionResult Index(string? q)
    {
        var model = new SearchResults { Query = q ?? "" };
        if (!string.IsNullOrWhiteSpace(q))
        {
            // Member + news search (parameterized — the SQLi lives in the comment section).
            model.Members = _db.Users
                .Where(u => u.FirstName.Contains(q) || u.LastName.Contains(q)
                            || u.Username.Contains(q) || (u.Title != null && u.Title.Contains(q)))
                .Take(25).ToList();
            model.Posts = _db.NewsPosts
                .Where(p => p.Title.Contains(q) || (p.Excerpt != null && p.Excerpt.Contains(q)))
                .Take(25).ToList();
        }
        return View(model);
    }
}
