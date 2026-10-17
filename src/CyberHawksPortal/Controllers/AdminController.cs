using System.Data;
using System.Diagnostics;
using System.Xml;
using CyberHawksPortal.Data;
using CyberHawksPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberHawksPortal.Controllers;

// VULN (unauthenticated admin interface #12 + missing function-level access control #7):
// this controller performs NO authentication or role checks whatsoever. /admin and every
// action under it are reachable by anyone who knows (or guesses) the URL. The only "security"
// is that the nav doesn't link here unless you're an admin. See SITE_PLAN.md §5 #7, #12.
[Route("admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminController(ApplicationDbContext db) => _db = db;

    [Route("")]
    public IActionResult Index()
    {
        ViewData["UserCount"] = _db.Users.Count();
        ViewData["CommentCount"] = _db.Comments.Count();
        ViewData["PostCount"] = _db.NewsPosts.Count();
        return View();
    }

    // Full user table, including password hashes and PII. (excessive exposure #11)
    [Route("users")]
    public IActionResult Users()
    {
        var users = _db.Users.OrderBy(u => u.UserId).ToList();
        return View(users);
    }

    // Comment moderation. (second-order / blind SQL injection #15)
    [Route("comments")]
    public IActionResult Comments()
    {
        var comments = _db.Comments.Include(c => c.Post)
            .OrderByDescending(c => c.CreatedAt).Take(100).ToList();

        // For each comment we show "how many comments this author has posted." The author name
        // was stored verbatim from the (parameterized) insert, but here it is concatenated
        // straight into a raw query — a classic second-order SQLi. A malformed stored value
        // throws, which surfaces to the attacker only as an HTTP 500 (the payload is never
        // reflected back on the public post page).
        var rows = new List<AdminCommentRow>();
        var conn = _db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open) conn.Open();
        foreach (var c in comments)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Comments WHERE AuthorName = '" + c.AuthorName + "'";
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            rows.Add(new AdminCommentRow { Comment = c, AuthorCommentCount = count });
        }
        return View(rows);
    }

    // Network diagnostics. (OS command injection #16)
    [HttpGet, Route("tools")]
    public IActionResult Tools() => View(model: (string?)null);

    [HttpPost, Route("tools")]
    public IActionResult Tools(string host)
    {
        string output;
        try
        {
            // Check whether a competition/ICS device is reachable. The host is dropped straight
            // into a shell command, so "127.0.0.1 & whoami" runs whoami too.
            var psi = new ProcessStartInfo("cmd.exe", "/c ping -n 2 " + host)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var p = Process.Start(psi)!;
            output = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
            p.WaitForExit(8000);
        }
        catch (Exception ex) { output = "Error: " + ex.Message; }
        ViewData["Host"] = host;
        return View(model: output);
    }

    // Roster import. (XML external entity injection #18)
    [HttpGet, Route("import")]
    public IActionResult Import() => View(model: (string?)null);

    [HttpPost, Route("import")]
    public IActionResult Import(string xml)
    {
        string result;
        try
        {
            // DTD processing + an external resolver are enabled, so an external entity in the
            // submitted XML (file:///… or http://10.0.2.x/…) is resolved and its content ends
            // up in the parsed document. Classic XXE.
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                XmlResolver = new XmlUrlResolver()
            };
            var doc = new XmlDocument { XmlResolver = new XmlUrlResolver() };
            using var sr = new StringReader(xml ?? "");
            using var reader = XmlReader.Create(sr, settings);
            doc.Load(reader);
            result = doc.InnerText;
        }
        catch (Exception ex) { result = "Error: " + ex.Message; }
        ViewData["Xml"] = xml;
        return View(model: result);
    }

    // Sponsor logo / link preview. (server-side request forgery #19)
    [HttpGet, Route("fetch")]
    public IActionResult Fetch() => View(model: (string?)null);

    [HttpPost, Route("fetch")]
    public async Task<IActionResult> Fetch(string url)
    {
        string result;
        try
        {
            // The server fetches whatever URL it is given, so it can be pointed at internal
            // hosts (10.0.2.x, cloud metadata at 169.254.169.254, localhost admin ports, …).
            using var resp = await Http.GetAsync(url);
            var body = await resp.Content.ReadAsStringAsync();
            result = $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}\n\n{body}";
        }
        catch (Exception ex) { result = "Error: " + ex.Message; }
        ViewData["Url"] = url;
        return View(model: result);
    }

    // Grant administrative access. (client-controlled authorization parameter #9)
    [HttpPost, Route("promote")]
    public IActionResult Promote(string username, string callerRole)
    {
        // "Authorization": we trust the role the CLIENT claims, instead of the session.
        if (callerRole != "officer" && callerRole != "admin")
            return Forbid();
        var u = _db.Users.FirstOrDefault(x => x.Username == username);
        if (u != null) { u.IsAdmin = true; u.Role = "Admin"; _db.SaveChanges(); }
        return RedirectToAction("Users");
    }

    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(8) };
}
