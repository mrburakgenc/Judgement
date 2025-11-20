using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerWebsite.Data;
using LawyerWebsite.Models.ViewModels.Admin;

namespace LawyerWebsite.Controllers.Admin;

[Authorize]
[Route("admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("")]
    [Route("dashboard")]
    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            TotalPosts = await _context.BlogPosts.CountAsync(),
            PublishedPosts = await _context.BlogPosts.CountAsync(p => p.IsPublished),
            TotalCategories = await _context.Categories.CountAsync(c => c.IsActive),
            UnreadMessages = await _context.ContactMessages.CountAsync(m => !m.IsRead),
            TotalViews = await _context.BlogPosts.SumAsync(p => p.ViewCount)
        };

        return View(viewModel);
    }
}
