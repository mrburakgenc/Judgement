using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerWebsite.Data;
using LawyerWebsite.Services;
using LawyerWebsite.Models.ViewModels;

namespace LawyerWebsite.Controllers;

public class BlogController : Controller
{
    private readonly IBlogService _blogService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BlogController> _logger;

    public BlogController(
        IBlogService blogService,
        ApplicationDbContext context,
        ILogger<BlogController> logger)
    {
        _blogService = blogService;
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? categoryId, int page = 1)
    {
        var posts = await _blogService.GetPublishedPostsAsync(categoryId);

        // Pagination
        int pageSize = 10;
        var totalPosts = posts.Count;
        var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);

        var pagedPosts = posts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentPage = page;
        ViewBag.CategoryId = categoryId;

        // Get categories for sidebar
        ViewBag.Categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return View(pagedPosts);
    }

    [Route("blog/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var post = await _blogService.GetPostBySlugAsync(slug);

        if (post == null || !post.IsPublished)
        {
            return NotFound();
        }

        // Increment view count
        await _blogService.IncrementViewCountAsync(post.Id);

        // Get related posts from same category
        var relatedPosts = await _context.BlogPosts
            .Where(p => p.IsPublished && p.CategoryId == post.CategoryId && p.Id != post.Id)
            .OrderByDescending(p => p.PublishedAt)
            .Take(3)
            .ToListAsync();

        // Get all categories for sidebar
        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        // Get documents for this blog post
        var documents = await _context.BlogPostDocuments
            .Where(d => d.BlogPostId == post.Id)
            .OrderBy(d => d.DisplayName)
            .ToListAsync();

        var viewModel = new BlogPostViewModel
        {
            Post = post,
            RelatedPosts = relatedPosts,
            AllCategories = categories
        };

        ViewBag.Documents = documents;

        return View(viewModel);
    }
}
