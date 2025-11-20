using System.ComponentModel.DataAnnotations;

namespace LawyerWebsite.Models.Entities;

public class SiteSetting
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string SettingKey { get; set; } = string.Empty;

    public string? SettingValue { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
