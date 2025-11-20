using System.ComponentModel.DataAnnotations;

namespace LawyerWebsite.Models.Entities;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
