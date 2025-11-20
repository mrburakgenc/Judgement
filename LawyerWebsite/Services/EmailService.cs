using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using LawyerWebsite.Models.Entities;

namespace LawyerWebsite.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendContactFormEmailAsync(ContactMessage message)
    {
        var emailBody = $@"
            <h2>Yeni İletişim Formu Mesajı</h2>
            <p><strong>Ad Soyad:</strong> {message.FullName}</p>
            <p><strong>E-posta:</strong> {message.Email}</p>
            <p><strong>Telefon:</strong> {message.Phone ?? "Belirtilmedi"}</p>
            <p><strong>Konu:</strong> {message.Subject ?? "Belirtilmedi"}</p>
            <p><strong>Mesaj:</strong></p>
            <p>{message.Message}</p>
            <p><small>Gönderim Tarihi: {message.CreatedAt:dd.MM.yyyy HH:mm}</small></p>
        ";

        var lawyerEmail = _configuration["EmailSettings:LawyerEmail"];
        if (string.IsNullOrEmpty(lawyerEmail))
        {
            _logger.LogWarning("Lawyer email address not configured");
            return;
        }

        await SendEmailAsync(lawyerEmail,
            $"Yeni İletişim Formu: {message.Subject ?? "Konu belirtilmedi"}",
            emailBody);
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"],
                _configuration["EmailSettings:SenderEmail"]
            ));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587"),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }
}
