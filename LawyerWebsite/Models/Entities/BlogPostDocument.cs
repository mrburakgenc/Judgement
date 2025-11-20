using System.ComponentModel.DataAnnotations;

namespace LawyerWebsite.Models.Entities;

public class BlogPostDocument
{
    public int Id { get; set; }

    public int BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; } = null!;

    [Required, MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? DisplayName { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public long FileSize { get; set; } // bytes

    [MaxLength(100)]
    public string FileType { get; set; } = string.Empty; // pdf, docx, xlsx, etc.

    public int DownloadCount { get; set; } = 0;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
