using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerWebsite.Data;
using LawyerWebsite.Models;

namespace LawyerWebsite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Get recent blog posts for home page
        var recentPosts = await _context.BlogPosts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .Take(3)
            .Include(p => p.Category)
            .ToListAsync();

        return View(recentPosts);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
