using CyberHawksPortal.Data;
using CyberHawksPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberHawksPortal.Controllers;

[Route("news")]
public class NewsController : Controller
{
    private readonly ApplicationDbContext _db;
    public NewsController(ApplicationDbContext db) => _db = db;

    [Route("")]
    public IActionResult Index()
    {
        var posts = _db.NewsPosts.Include(p => p.Author).AsNoTracking()
            .OrderByDescending(p => p.IsPinned).ThenByDescending(p => p.PublishedAt)
            .ToList();
        return View(posts);
    }

    [Route("{slug}")]
    public IActionResult Details(string slug)
    {
        var post = _db.NewsPosts.Include(p => p.Author).Include(p => p.Comments)
            .AsNoTracking().FirstOrDefault(p => p.Slug == slug);
        if (post == null) return NotFound();
        return View(post);
    }

    [HttpPost]
    [Route("{slug}/comment")]
    public IActionResult Comment(string slug, string authorName, string body)
    {
        var post = _db.NewsPosts.FirstOrDefault(p => p.Slug == slug);
        if (post == null) return NotFound();
        if (!string.IsNullOrWhiteSpace(body))
        {
            _db.Comments.Add(new Comment
            {
                NewsPostId = post.NewsPostId,
                AuthorName = string.IsNullOrWhiteSpace(authorName) ? "Anonymous Hawk" : authorName,
                Body = body
            });
            _db.SaveChanges();
        }
        return RedirectToAction("Details", new { slug });
    }
}
