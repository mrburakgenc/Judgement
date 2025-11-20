using Microsoft.EntityFrameworkCore;
using LawyerWebsite.Data;
using LawyerWebsite.Models.Entities;

namespace LawyerWebsite.Services;

public class BlogService : IBlogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BlogService> _logger;

    public BlogService(ApplicationDbContext context, ILogger<BlogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BlogPost>> GetPublishedPostsAsync(int? categoryId = null)
    {
        var query = _context.BlogPosts
            .Include(p => p.Category)
            .Where(p => p.IsPublished);

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        return await _context.BlogPosts
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<BlogPost?> GetPostByIdAsync(int id)
    {
        return await _context.BlogPosts
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<BlogPost> CreatePostAsync(BlogPost post)
    {
        _context.BlogPosts.Add(post);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Blog post created: {Title}", post.Title);
        return post;
    }

    public async Task UpdatePostAsync(BlogPost post)
    {
        post.UpdatedAt = DateTime.UtcNow;
        _context.BlogPosts.Update(post);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Blog post updated: {Title}", post.Title);
    }

    public async Task DeletePostAsync(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post != null)
        {
            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Blog post deleted: {Id}", id);
        }
    }

    public async Task IncrementViewCountAsync(int postId)
    {
        var post = await _context.BlogPosts.FindAsync(postId);
        if (post != null)
        {
            post.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
}
