using CyberHawksPortal.Data;
using Microsoft.AspNetCore.Mvc;

namespace CyberHawksPortal.Controllers;

[Route("api")]
public class ApiController : Controller
{
    private readonly ApplicationDbContext _db;
    public ApiController(ApplicationDbContext db) => _db = db;

    // VULN (CORS misconfiguration #26): the request Origin is reflected and credentials are
    // allowed, so any site can read these authenticated responses in a victim's browser.
    private void Cors()
    {
        var origin = Request.Headers["Origin"].ToString();
        if (!string.IsNullOrEmpty(origin))
        {
            Response.Headers["Access-Control-Allow-Origin"] = origin;
            Response.Headers["Access-Control-Allow-Credentials"] = "true";
        }
    }

    // VULN (excessive data exposure #11): returns the full member records — password hashes,
    // SSNs and stipend amounts included — to anyone.
    [Route("members")]
    public IActionResult Members()
    {
        Cors();
        return Json(_db.Users.OrderBy(u => u.UserId).ToList());
    }

    // VULN (IDOR #8 + excessive exposure #11): any id returns that member's full private record,
    // with no authentication or ownership check.
    [Route("members/{id:int}")]
    public IActionResult Member(int id)
    {
        Cors();
        var u = _db.Users.FirstOrDefault(x => x.UserId == id);
        if (u == null) return NotFound(new { error = "No such member." });
        return Json(u);
    }
}
