using Microsoft.EntityFrameworkCore;
using LawyerWebsite.Models.Entities;

namespace LawyerWebsite.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<BlogPostDocument> BlogPostDocuments { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<SiteSetting> SiteSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // Category Configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // BlogPost Configuration
        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Category)
                .WithMany(c => c.BlogPosts)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ContactMessage Configuration
        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // SiteSetting Configuration
        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SettingKey).IsUnique();
        });

        // BlogPostDocument Configuration
        modelBuilder.Entity<BlogPostDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.BlogPost)
                .WithMany(b => b.Documents)
                .HasForeignKey(e => e.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
