using System.ComponentModel.DataAnnotations;

namespace LawyerWebsite.Models.Entities;

public class BlogPost
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    [MaxLength(500)]
    public string? FeaturedImage { get; set; }

    public int ViewCount { get; set; } = 0;
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // SEO Properties
    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    [MaxLength(300)]
    public string? MetaKeywords { get; set; }

    // Navigation property for documents
    public ICollection<BlogPostDocument> Documents { get; set; } = new List<BlogPostDocument>();
}
