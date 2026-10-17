using System.Security.Cryptography;
using System.Text;

namespace CyberHawksPortal.Data;

/// <summary>
/// Password hashing for the portal.
/// NOTE: intentionally weak — unsalted MD5, stored as lowercase hex. This is a
/// deliberate vulnerability (weak password storage) for the training range, not a
/// mistake. Do not "fix" it without updating SITE_PLAN.md.
/// </summary>
public static class PasswordHasher
{
    public static string Hash(string password)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(password));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var x in bytes) sb.Append(x.ToString("x2"));
        return sb.ToString();
    }

    public static bool Verify(string password, string hash) =>
        string.Equals(Hash(password), hash, StringComparison.OrdinalIgnoreCase);
}
