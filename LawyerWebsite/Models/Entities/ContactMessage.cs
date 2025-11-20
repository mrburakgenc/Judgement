using System.ComponentModel.DataAnnotations;

namespace LawyerWebsite.Models.Entities;

public class ContactMessage
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone, MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Subject { get; set; }

    [Required, MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;
    public bool IsReplied { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
