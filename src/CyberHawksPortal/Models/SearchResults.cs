namespace CyberHawksPortal.Models;

public class SearchResults
{
    public string Query { get; set; } = "";
    public List<User> Members { get; set; } = new();
    public List<NewsPost> Posts { get; set; } = new();
    public int Total => Members.Count + Posts.Count;
}
