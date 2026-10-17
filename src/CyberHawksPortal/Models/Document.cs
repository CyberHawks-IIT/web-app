namespace CyberHawksPortal.Models;

public class Document
{
    public int DocumentId { get; set; }
    public string Title { get; set; } = "";
    public string Category { get; set; } = "General";
    public string FileName { get; set; } = "";
    public string? Description { get; set; }
    public string UploadedBy { get; set; } = "CyberHawks";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
