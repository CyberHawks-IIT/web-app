using System.Text;
using CyberHawksPortal.Data;
using CyberHawksPortal.Models;
using CyberHawksPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberHawksPortal.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;
    public AccountController(ApplicationDbContext db) => _db = db;

    // ---------------- Login ----------------
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var user = _db.Users.FirstOrDefault(u => u.Username == username);

        // VULN (username enumeration, SITE_PLAN §5 #1): distinct messages for
        // "no such user" vs "wrong password" let an attacker enumerate valid usernames.
        if (user == null)
        {
            ViewData["Error"] = $"No account exists with the username \"{username}\".";
            return View();
        }
        if (!PasswordHasher.Verify(password, user.PasswordHash))
        {
            ViewData["Error"] = "That password is incorrect. The Overlord is watching, but patiently.";
            return View();
        }

        // VULN (forgeable session token #6): unsigned Base64(userId:role) cookie.
        IssueSession(user);

        // VULN (open redirect #27): returnUrl is redirected to without validation.
        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Dashboard");
    }

    // ---------------- Register ----------------
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    // VULN (mass assignment #10): the whole User entity is model-bound from the form, so a
    // crafted POST can set Role / IsAdmin / MembershipLevel / StipendAmount, etc.
    public IActionResult Register(User form, string password)
    {
        if (string.IsNullOrWhiteSpace(form.Username) || string.IsNullOrWhiteSpace(password))
        {
            ViewData["Error"] = "A username and password are required. We don't ask for much.";
            return View(form);
        }
        if (_db.Users.Any(u => u.Username == form.Username))
        {
            ViewData["Error"] = "That username is already venerating on someone else's behalf. Pick another.";
            return View(form);
        }

        // VULN (weak password policy #3): NO server-side strength check — "1" is accepted.
        // VULN (weak password storage #30): unsalted MD5.
        form.UserId = 0;
        form.PasswordHash = PasswordHasher.Hash(password);
        form.CreatedAt = DateTime.UtcNow;
        if (string.IsNullOrEmpty(form.Role)) form.Role = "Member";
        _db.Users.Add(form);
        _db.SaveChanges();

        IssueSession(form);
        return RedirectToAction("Index", "Dashboard");
    }

    // ---------------- Logout ----------------
    public IActionResult Logout()
    {
        Response.Cookies.Delete(SessionToken.CookieName);
        return RedirectToAction("Index", "Home");
    }

    // ---------------- Forgot / reset password ----------------
    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public IActionResult ForgotPassword(string username)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        // VULN (username enumeration #1, again) + (insecure password reset #4):
        // the reset token is PREDICTABLE — Base64("userId:yyyyMMdd") — with no expiry or
        // rate limit, so anyone can forge a valid reset link for any account.
        if (user == null)
        {
            ViewData["Error"] = $"We have no record of \"{username}\". No reset was sent.";
            return View();
        }
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.UserId}:{DateTime.UtcNow:yyyyMMdd}"));
        // Our mail server is, regrettably, on sabbatical. Here is your link directly:
        ViewData["ResetLink"] = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);
        ViewData["Ok"] = $"A reset link for \"{username}\" has been generated.";
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        ViewData["Token"] = token;
        return View();
    }

    [HttpPost]
    [ActionName("ResetPassword")]
    public IActionResult ResetPasswordPost(string token, string password)
    {
        ViewData["Token"] = token;
        int userId = 0;
        try
        {
            var raw = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            int.TryParse(raw.Split(':')[0], out userId);
        }
        catch { }

        var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            ViewData["Error"] = "That reset token is not valid.";
            return View();
        }
        // No expiry check, no rate limit, token never invalidated.
        user.PasswordHash = PasswordHasher.Hash(password);
        _db.SaveChanges();
        ViewData["Ok"] = "Password updated. You may now sign in and resume venerating.";
        return View();
    }

    private void IssueSession(User user)
    {
        Response.Cookies.Append(SessionToken.CookieName, SessionToken.Issue(user), new CookieOptions
        {
            HttpOnly = false,   // deliberately readable by JS (lab)
            Secure = false,     // plain HTTP (lab)
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }
}
