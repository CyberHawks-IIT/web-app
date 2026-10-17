using CyberHawksPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CyberHawksPortal.Controllers;

[Route("documents")]
public class DocumentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    public DocumentsController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db; _env = env;
    }

    private string StoreDir()
    {
        var dir = Path.Combine(_env.ContentRootPath, "FileStore");
        Directory.CreateDirectory(dir);
        // make sure the seeded sample files exist so legitimate downloads work
        foreach (var d in _db.Documents.ToList())
        {
            var p = Path.Combine(dir, d.FileName);
            if (!System.IO.File.Exists(p))
                System.IO.File.WriteAllText(p, $"{d.Title}\n{new string('=', d.Title.Length)}\n\n{d.Description}\n\nUploaded by {d.UploadedBy}.\n");
        }
        return dir;
    }

    [Route("")]
    public IActionResult Index()
    {
        StoreDir();
        var docs = _db.Documents.OrderByDescending(d => d.CreatedAt).ToList();
        return View(docs);
    }

    // VULN (path traversal #21): the file name is combined with the store directory with no
    // validation, so "?file=../../appsettings.json" (or ../../Web.config.bak, etc.) escapes it.
    [Route("download")]
    public IActionResult Download(string file)
    {
        var dir = StoreDir();
        var path = Path.Combine(dir, file ?? "");
        if (!System.IO.File.Exists(path)) return NotFound("File not found.");
        var bytes = System.IO.File.ReadAllBytes(path);
        var ctype = ContentTypeFor(path);
        // inline so an uploaded .html renders (and runs) in the browser
        return File(bytes, ctype);
    }

    [HttpGet, Route("upload")]
    public IActionResult Upload() => View();

    [HttpPost, Route("upload")]
    // VULN (file upload #20): the client blocks .html via the accept attribute + JS, but the
    // SERVER performs no extension/content-type check, so an .html (or .aspx, .svg, …) can be
    // uploaded and is then served inline from /documents/download — stored XSS / content injection.
    public async Task<IActionResult> Upload(IFormFile upload, string? title)
    {
        if (upload == null || upload.Length == 0)
        {
            ViewData["Error"] = "Please choose a file to upload.";
            return View();
        }
        var dir = StoreDir();
        var name = Path.GetFileName(upload.FileName);
        var dest = Path.Combine(dir, name);
        using (var fs = new FileStream(dest, FileMode.Create))
            await upload.CopyToAsync(fs);

        _db.Documents.Add(new Models.Document
        {
            Title = string.IsNullOrWhiteSpace(title) ? name : title,
            Category = "Uploads",
            FileName = name,
            Description = "Member-uploaded document.",
            UploadedBy = HttpContext.Items["CurrentUser"] is Models.User u ? u.FullName : "A Member"
        });
        _db.SaveChanges();
        ViewData["Ok"] = $"Uploaded \"{name}\". The Office of Veneration thanks you for your contribution.";
        return View();
    }

    private static string ContentTypeFor(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".html" or ".htm" => "text/html",
            ".svg" => "image/svg+xml",
            ".txt" or ".log" or ".md" => "text/plain",
            ".json" => "application/json",
            ".config" or ".bak" or ".xml" => "application/xml",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}
