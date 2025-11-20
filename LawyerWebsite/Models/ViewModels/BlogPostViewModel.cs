using LawyerWebsite.Models.Entities;

namespace LawyerWebsite.Models.ViewModels;

public class BlogPostViewModel
{
    public BlogPost Post { get; set; } = new();
    public List<BlogPost> RelatedPosts { get; set; } = new();
    public List<Category> AllCategories { get; set; } = new();
}
