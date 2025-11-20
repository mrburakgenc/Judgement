using LawyerWebsite.Models.Entities;

namespace LawyerWebsite.Services;

public interface IEmailService
{
    Task SendContactFormEmailAsync(ContactMessage message);
    Task SendEmailAsync(string to, string subject, string body);
}
