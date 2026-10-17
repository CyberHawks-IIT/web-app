namespace CyberHawksPortal.Models;

public class Comment
{
    public int CommentId { get; set; }
    public int NewsPostId { get; set; }
    public NewsPost? Post { get; set; }

    public string AuthorName { get; set; } = "";
    public string Body { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
