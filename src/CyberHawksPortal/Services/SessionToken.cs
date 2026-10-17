using System.Text;
using CyberHawksPortal.Models;

namespace CyberHawksPortal.Services;

/// <summary>
/// Issues and reads the "CHAUTH" session cookie.
/// NOTE (intentional vulnerability — predictable/forgeable session token): the token is just a
/// Base64 encoding of "userId:role" with NO signature, so anyone can decode it, change the user
/// id or role, re-encode it, and impersonate another member (or an admin). This is deliberate
/// for the training range — see SITE_PLAN.md §5 #6. Do not add a real signature.
/// </summary>
public static class SessionToken
{
    public const string CookieName = "CHAUTH";

    public static string Issue(User u)
    {
        var raw = $"{u.UserId}:{u.Role}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
    }

    public static bool TryRead(string? cookie, out int userId, out string role)
    {
        userId = 0; role = "Member";
        if (string.IsNullOrWhiteSpace(cookie)) return false;
        try
        {
            var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cookie));
            var parts = raw.Split(':', 2);
            if (parts.Length < 1 || !int.TryParse(parts[0], out userId)) return false;
            role = parts.Length > 1 ? parts[1] : "Member";
            return true;
        }
        catch { return false; }
    }
}
