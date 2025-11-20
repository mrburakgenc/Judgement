using LawyerWebsite.Models.Entities;

namespace LawyerWebsite.Services;

public interface IBlogService
{
    Task<List<BlogPost>> GetPublishedPostsAsync(int? categoryId = null);
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task<BlogPost?> GetPostByIdAsync(int id);
    Task<BlogPost> CreatePostAsync(BlogPost post);
    Task UpdatePostAsync(BlogPost post);
    Task DeletePostAsync(int id);
    Task IncrementViewCountAsync(int postId);
}
