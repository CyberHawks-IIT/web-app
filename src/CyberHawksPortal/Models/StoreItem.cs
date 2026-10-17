namespace CyberHawksPortal.Models;

public class StoreItem
{
    public int StoreItemId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int Cost { get; set; }       // in Hawk Points
    public int Stock { get; set; }
}

public class Redemption
{
    public int RedemptionId { get; set; }
    public int UserId { get; set; }
    public string Reward { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
