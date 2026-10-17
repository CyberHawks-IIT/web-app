namespace CyberHawksPortal.Models;

public class AdminCommentRow
{
    public Comment Comment { get; set; } = null!;
    public int AuthorCommentCount { get; set; }
}
