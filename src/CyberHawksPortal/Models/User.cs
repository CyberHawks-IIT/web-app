namespace CyberHawksPortal.Models;

/// <summary>
/// A CyberHawks member. Backed by the dbo.Users table on sql1.
/// </summary>
public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Email { get; set; } = "";

    // "Member", "Officer", "Admin"
    public string Role { get; set; } = "Member";
    public bool IsAdmin { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? Title { get; set; }           // officer title / tagline
    public string? Headline { get; set; }        // short one-liner on the profile
    public string? Bio { get; set; }
    public string? Highlights { get; set; }      // newline-separated achievement bullets
    public string? Handle { get; set; }          // e.g. darnold, redefiningreality
    public string? AvatarPath { get; set; }
    public bool IsFounder { get; set; }
    public bool IsLegend { get; set; }           // hall-of-fame alumni

    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Ssn { get; set; }              // stored unencrypted (intentional)
    public decimal? StipendAmount { get; set; }   // sensitive (intentional)

    public string MembershipLevel { get; set; } = "Student";
    public int? GradYear { get; set; }
    public int Points { get; set; } = 100;        // "Hawk Points" spendable in the store
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Initials =>
        $"{(FirstName.Length > 0 ? FirstName[0] : 'C')}{(LastName.Length > 0 ? LastName[0] : 'H')}";

    public string[] HighlightList =>
        string.IsNullOrWhiteSpace(Highlights)
            ? System.Array.Empty<string>()
            : Highlights.Split('\n', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
}
