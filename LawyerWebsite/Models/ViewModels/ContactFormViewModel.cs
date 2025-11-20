using System.ComponentModel.DataAnnotations;

namespace LawyerWebsite.Models.ViewModels;

public class ContactFormViewModel
{
    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    [MaxLength(100)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    [MaxLength(100)]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    [MaxLength(20)]
    [Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [MaxLength(200)]
    [Display(Name = "Konu")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Mesaj zorunludur")]
    [MaxLength(2000)]
    [Display(Name = "Mesajınız")]
    public string Message { get; set; } = string.Empty;
}
