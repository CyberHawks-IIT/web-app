using CyberHawksPortal.Data;
using CyberHawksPortal.Models;
using CyberHawksPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberHawksPortal.Controllers;

[Route("profile")]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _db;
    public ProfileController(ApplicationDbContext db) => _db = db;

    [HttpGet, Route("edit")]
    public IActionResult Edit()
    {
        var user = HttpContext.CurrentUser();
        if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = "/profile/edit" });
        return View(user);
    }

    [HttpPost, Route("edit")]
    // VULN (CSRF #25): no antiforgery validation — a cross-site POST can change this account.
    // VULN (mass assignment #10): the posted form is copied field-by-field, Role / IsAdmin
    // / MembershipLevel / StipendAmount included, so a member can POST IsAdmin=true to elevate.
    public IActionResult Edit(User form)
    {
        var user = HttpContext.CurrentUser();
        if (user == null) return RedirectToAction("Login", "Account");

        user.FirstName = form.FirstName;
        user.LastName = form.LastName;
        user.Email = form.Email;
        user.Title = form.Title;
        user.Headline = form.Headline;
        user.Bio = form.Bio;                       // rendered unencoded on the profile → stored XSS
        user.AvatarPath = form.AvatarPath;
        user.Role = form.Role ?? user.Role;        // over-postable
        user.IsAdmin = form.IsAdmin;               // over-postable
        user.MembershipLevel = form.MembershipLevel ?? user.MembershipLevel;
        user.StipendAmount = form.StipendAmount ?? user.StipendAmount;
        _db.SaveChanges();

        return RedirectToAction("Details", "Members", new { id = user.UserId });
    }

    [HttpPost, Route("delete")]
    public IActionResult Delete()
    {
        var user = HttpContext.CurrentUser();
        if (user != null)
        {
            _db.Users.Remove(user);
            _db.SaveChanges();
            Response.Cookies.Delete(SessionToken.CookieName);
        }
        return RedirectToAction("Index", "Home");
    }
}
