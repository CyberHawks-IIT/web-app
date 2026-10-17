namespace CyberHawksPortal.Models;

public class NewsPost
{
    public int NewsPostId { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Category { get; set; } = "News";   // News, Seminar, Competition, Spotlight
    public string? Excerpt { get; set; }
    public string Body { get; set; } = "";
    public string? ImagePath { get; set; }
    public bool IsPinned { get; set; }

    public int AuthorId { get; set; }
    public User? Author { get; set; }

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    public List<Comment> Comments { get; set; } = new();
}
