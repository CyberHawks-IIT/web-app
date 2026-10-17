using CyberHawksPortal.Data;
using CyberHawksPortal.Models;
using CyberHawksPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberHawksPortal.Controllers;

[Route("store")]
public class StoreController : Controller
{
    private readonly ApplicationDbContext _db;
    public StoreController(ApplicationDbContext db) => _db = db;

    [Route("")]
    public IActionResult Index()
    {
        var user = HttpContext.CurrentUser();
        if (user == null) return RedirectToAction("Login", "Account", new { returnUrl = "/store" });
        ViewData["Items"] = _db.StoreItems.OrderBy(i => i.StoreItemId).ToList();
        ViewData["Vouchers"] = _db.Redemptions.Count(r => r.UserId == user.UserId);
        return View(user);
    }

    // One-time welcome voucher.
    // VULN (race condition #14): the "have you already redeemed?" check and the insert are not
    // atomic and there is a deliberate window between them, so firing several requests at once
    // slips past the check and mints multiple vouchers. See SITE_PLAN.md §5 #14.
    [HttpPost, Route("redeem")]
    public IActionResult Redeem()
    {
        var user = HttpContext.CurrentUser();
        if (user == null) return RedirectToAction("Login", "Account");

        if (_db.Redemptions.Any(r => r.UserId == user.UserId))
        {
            TempData["Msg"] = "You have already claimed your welcome voucher.";
            return RedirectToAction("Index");
        }

        System.Threading.Thread.Sleep(200); // check-then-act window

        _db.Redemptions.Add(new Redemption { UserId = user.UserId, Reward = "Free Nationals Hoodie" });
        _db.SaveChanges();
        TempData["Msg"] = "🎁 Welcome voucher claimed — one free Nationals Hoodie!";
        return RedirectToAction("Index");
    }

    // Spend Hawk Points on an item.
    // VULN (business logic flaw #13): quantity is trusted from the client with no sanity checks —
    // a negative quantity yields a negative cost (so you *gain* points), there is no check that
    // you can afford it, and stock is not guarded. See SITE_PLAN.md §5 #13.
    [HttpPost, Route("buy")]
    public IActionResult Buy(int itemId, int quantity)
    {
        var user = HttpContext.CurrentUser();
        if (user == null) return RedirectToAction("Login", "Account");
        var item = _db.StoreItems.FirstOrDefault(i => i.StoreItemId == itemId);
        if (item == null) return RedirectToAction("Index");

        var cost = item.Cost * quantity;
        user.Points -= cost;        // negative quantity -> negative cost -> points increase
        item.Stock -= quantity;
        _db.SaveChanges();

        TempData["Msg"] = $"Order processed: {quantity} × {item.Name} for {cost} points. Balance: {user.Points}.";
        return RedirectToAction("Index");
    }
}
