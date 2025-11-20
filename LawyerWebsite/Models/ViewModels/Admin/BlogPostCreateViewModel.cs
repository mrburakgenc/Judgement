using System.ComponentModel.DataAnnotations;

namespace LawyerWebsite.Models.ViewModels.Admin;

public class BlogPostCreateViewModel
{
    [Required(ErrorMessage = "Başlık zorunludur")]
    [MaxLength(200)]
    [Display(Name = "Başlık")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Özet")]
    public string? Summary { get; set; }

    [Required(ErrorMessage = "İçerik zorunludur")]
    [Display(Name = "İçerik")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Kategori")]
    public int? CategoryId { get; set; }

    [Display(Name = "Öne Çıkan Görsel")]
    public IFormFile? FeaturedImageFile { get; set; }

    [Display(Name = "Yayınla")]
    public bool IsPublished { get; set; } = false;

    [MaxLength(200)]
    [Display(Name = "SEO Başlık")]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    [Display(Name = "SEO Açıklama")]
    public string? MetaDescription { get; set; }

    [MaxLength(300)]
    [Display(Name = "SEO Anahtar Kelimeler")]
    public string? MetaKeywords { get; set; }

    [Display(Name = "Evraklar / Dökümanlar")]
    public List<IFormFile>? DocumentFiles { get; set; }
}
