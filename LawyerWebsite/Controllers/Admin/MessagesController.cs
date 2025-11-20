using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerWebsite.Data;

namespace LawyerWebsite.Controllers.Admin;

[Authorize]
[Route("admin/messages")]
public class MessagesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(ApplicationDbContext context, ILogger<MessagesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        var messages = await _context.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return View(messages);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null)
        {
            return NotFound();
        }

        // Mark as read
        if (!message.IsRead)
        {
            message.IsRead = true;
            await _context.SaveChangesAsync();
        }

        return View(message);
    }

    [HttpPost("mark-replied/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkReplied(int id)
    {
        try
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                message.IsReplied = true;
                message.IsRead = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mesaj cevaplanmış olarak işaretlendi.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as replied");
            TempData["Error"] = "İşlem sırasında bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mesaj başarıyla silindi.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message");
            TempData["Error"] = "Mesaj silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }
}
