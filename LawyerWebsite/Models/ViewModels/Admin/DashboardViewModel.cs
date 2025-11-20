namespace LawyerWebsite.Models.ViewModels.Admin;

public class DashboardViewModel
{
    public int TotalPosts { get; set; }
    public int PublishedPosts { get; set; }
    public int TotalCategories { get; set; }
    public int UnreadMessages { get; set; }
    public int TotalViews { get; set; }
}
