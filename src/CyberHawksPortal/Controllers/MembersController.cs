using CyberHawksPortal.Data;
using CyberHawksPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberHawksPortal.Controllers;

[Route("members")]
public class MembersController : Controller
{
    private readonly ApplicationDbContext _db;
    public MembersController(ApplicationDbContext db) => _db = db;

    [Route("")]
    public IActionResult Index()
    {
        var users = _db.Users.AsNoTracking().ToList();
        return View(users);
    }

    [Route("{id:int}")]
    public IActionResult Details(int id)
    {
        var user = _db.Users.AsNoTracking().FirstOrDefault(u => u.UserId == id);
        if (user == null) return NotFound();
        return View(user);
    }
}
