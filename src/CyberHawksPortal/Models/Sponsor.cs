namespace CyberHawksPortal.Models;

public class Sponsor
{
    public int SponsorId { get; set; }
    public string Name { get; set; } = "";
    public string Tier { get; set; } = "Partner";   // Platinum, Gold, Partner
    public string? Blurb { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LogoUrl { get; set; }
    public int SortOrder { get; set; }
}
