using System.Text;
using CyberHawksPortal.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CyberHawksPortal.Controllers;

public class UserPreferences
{
    public string Theme { get; set; } = "scarlet";
    public int ResultsPerPage { get; set; } = 10;
    public bool ShowVenerationIndex { get; set; } = true;
}

[Route("preferences")]
public class PreferencesController : Controller
{
    private const string CookieName = "CHPREFS";

    // VULN (insecure deserialization #17): preferences are stored in a cookie as Base64 of
    // Json.NET output written with TypeNameHandling.All, then deserialized with the same setting.
    // An attacker who crafts the cookie (e.g. with a ysoserial.net .NET gadget) achieves code
    // execution when the cookie is read. See SITE_PLAN.md §5 #17.
    private static readonly JsonSerializerSettings VulnSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    [Route("")]
    public IActionResult Index()
    {
        var prefs = ReadPrefs() ?? new UserPreferences();
        return View(prefs);
    }

    [HttpPost, Route("")]
    public IActionResult Index(UserPreferences prefs)
    {
        var json = JsonConvert.SerializeObject(prefs, VulnSettings);
        var cookie = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        Response.Cookies.Append(CookieName, cookie, new CookieOptions { Path = "/", HttpOnly = false });
        ViewData["Ok"] = "Preferences saved. Your experience is now 12% more venerable.";
        return View(prefs);
    }

    private UserPreferences? ReadPrefs()
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var cookie) || string.IsNullOrEmpty(cookie))
            return null;
        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(cookie));
            return JsonConvert.DeserializeObject<UserPreferences>(json, VulnSettings);
        }
        catch { return null; }
    }
}
