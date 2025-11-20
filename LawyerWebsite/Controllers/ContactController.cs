using Microsoft.AspNetCore.Mvc;
using LawyerWebsite.Data;
using LawyerWebsite.Services;
using LawyerWebsite.Models.Entities;
using LawyerWebsite.Models.ViewModels;

namespace LawyerWebsite.Controllers;

public class ContactController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(
        ApplicationDbContext context,
        IEmailService emailService,
        ILogger<ContactController> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Lütfen tüm alanları doğru şekilde doldurun.";
            return Redirect("/#section-contact");
        }

        try
        {
            var message = new ContactMessage
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Subject = model.Subject,
                Message = model.Message,
                CreatedAt = DateTime.UtcNow
            };

            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();

            // Send email
            try
            {
                await _emailService.SendContactFormEmailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact form email");
                // Don't fail the request if email sending fails
            }

            TempData["Success"] = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapılacaktır.";
            return Redirect("/#section-contact");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving contact message");
            TempData["Error"] = "Mesaj gönderilirken bir hata oluştu. Lütfen tekrar deneyin.";
            return Redirect("/#section-contact");
        }
    }
}
