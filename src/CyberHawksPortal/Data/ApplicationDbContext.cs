using CyberHawksPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CyberHawksPortal.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<NewsPost> NewsPosts => Set<NewsPost>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Sponsor> Sponsors => Set<Sponsor>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<StoreItem> StoreItems => Set<StoreItem>();
    public DbSet<Redemption> Redemptions => Set<Redemption>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.StipendAmount).HasColumnType("decimal(10,2)");
        });

        b.Entity<NewsPost>(e =>
        {
            e.ToTable("NewsPosts");
            e.HasOne(p => p.Author).WithMany().HasForeignKey(p => p.AuthorId).OnDelete(DeleteBehavior.NoAction);
        });

        b.Entity<Comment>(e =>
        {
            e.ToTable("Comments");
            e.HasOne(c => c.Post).WithMany(p => p.Comments).HasForeignKey(c => c.NewsPostId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Sponsor>().ToTable("Sponsors");
        b.Entity<Document>().ToTable("Documents");
        b.Entity<StoreItem>().ToTable("StoreItems");
        b.Entity<Redemption>().ToTable("Redemptions");
    }
}
