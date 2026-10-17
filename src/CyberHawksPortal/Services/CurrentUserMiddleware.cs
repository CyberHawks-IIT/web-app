using CyberHawksPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace CyberHawksPortal.Services;

/// <summary>
/// Reads the CHAUTH cookie on every request and puts the corresponding member on
/// HttpContext.Items["CurrentUser"] so views/controllers can see who is signed in.
/// The cookie is trusted as-is (see SessionToken for the intentional weakness).
/// </summary>
public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;
    public CurrentUserMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext ctx, ApplicationDbContext db)
    {
        if (ctx.Request.Cookies.TryGetValue(SessionToken.CookieName, out var cookie)
            && SessionToken.TryRead(cookie, out var userId, out _))
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null) ctx.Items["CurrentUser"] = user;
        }
        await _next(ctx);
    }
}

public static class HttpContextExtensions
{
    public static CyberHawksPortal.Models.User? CurrentUser(this HttpContext ctx)
        => ctx.Items["CurrentUser"] as CyberHawksPortal.Models.User;
}
