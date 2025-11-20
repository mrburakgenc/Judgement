using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerWebsite.Data;
using LawyerWebsite.Services;
using LawyerWebsite.Models.Entities;
using LawyerWebsite.Models.ViewModels.Admin;
using LawyerWebsite.Helpers;

namespace LawyerWebsite.Controllers.Admin;

[Authorize]
[Route("admin/blog")]
public class BlogManagementController : Controller
{
    private readonly IBlogService _blogService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BlogManagementController> _logger;
    private readonly IWebHostEnvironment _environment;

    public BlogManagementController(
        IBlogService blogService,
        ApplicationDbContext context,
        ILogger<BlogManagementController> logger,
        IWebHostEnvironment environment)
    {
        _blogService = blogService;
        _context = context;
        _logger = logger;
        _environment = environment;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        var posts = await _context.BlogPosts
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(posts);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        ViewBag.Categories = _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToList();

        return View();
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPostCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
            return View(model);
        }

        try
        {
            var post = new BlogPost
            {
                Title = model.Title,
                Slug = SlugHelper.GenerateSlug(model.Title),
                Summary = model.Summary,
                Content = model.Content,
                CategoryId = model.CategoryId,
                IsPublished = model.IsPublished,
                PublishedAt = model.IsPublished ? DateTime.UtcNow : null,
                CreatedAt = DateTime.UtcNow,
                MetaTitle = model.MetaTitle ?? model.Title,
                MetaDescription = model.MetaDescription ?? model.Summary,
                MetaKeywords = model.MetaKeywords
            };

            // Handle image upload
            if (model.FeaturedImageFile != null)
            {
                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "blog");
                var fileName = await ImageHelper.SaveImageAsync(model.FeaturedImageFile, uploadFolder);
                post.FeaturedImage = $"/uploads/blog/{fileName}";
            }

            await _blogService.CreatePostAsync(post);

            // Handle document uploads
            if (model.DocumentFiles != null && model.DocumentFiles.Any())
            {
                var documentUploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "documents");

                foreach (var file in model.DocumentFiles)
                {
                    if (file != null && DocumentHelper.IsValidDocument(file))
                    {
                        try
                        {
                            var (fileName, filePath) = await DocumentHelper.SaveDocumentAsync(file, documentUploadFolder);

                            var document = new BlogPostDocument
                            {
                                BlogPostId = post.Id,
                                FileName = fileName,
                                FilePath = $"/uploads/documents/{fileName}",
                                DisplayName = file.FileName,
                                FileSize = file.Length,
                                FileType = Path.GetExtension(file.FileName).ToLowerInvariant(),
                                UploadedAt = DateTime.UtcNow
                            };

                            _context.BlogPostDocuments.Add(document);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error uploading document {FileName}", file.FileName);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Blog yazısı başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating blog post");
            ModelState.AddModelError("", "Blog yazısı oluşturulurken bir hata oluştu.");

            ViewBag.Categories = _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToList();

            return View(model);
        }
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _blogService.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        var model = new BlogPostEditViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Summary = post.Summary,
            Content = post.Content,
            CategoryId = post.CategoryId,
            CurrentFeaturedImage = post.FeaturedImage,
            IsPublished = post.IsPublished,
            MetaTitle = post.MetaTitle,
            MetaDescription = post.MetaDescription,
            MetaKeywords = post.MetaKeywords
        };

        ViewBag.Categories = _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToList();

        // Load documents for this blog post
        ViewBag.Documents = await _context.BlogPostDocuments
            .Where(d => d.BlogPostId == id)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

        return View(model);
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogPostEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
            return View(model);
        }

        try
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.Title = model.Title;
            post.Slug = SlugHelper.GenerateSlug(model.Title);
            post.Summary = model.Summary;
            post.Content = model.Content;
            post.CategoryId = model.CategoryId;
            post.IsPublished = model.IsPublished;
            post.MetaTitle = model.MetaTitle ?? model.Title;
            post.MetaDescription = model.MetaDescription ?? model.Summary;
            post.MetaKeywords = model.MetaKeywords;

            if (!post.IsPublished)
            {
                post.PublishedAt = null;
            }
            else if (post.PublishedAt == null)
            {
                post.PublishedAt = DateTime.UtcNow;
            }

            // Handle image upload
            if (model.FeaturedImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(post.FeaturedImage))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, post.FeaturedImage.TrimStart('/'));
                    ImageHelper.DeleteImage(oldImagePath);
                }

                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "blog");
                var fileName = await ImageHelper.SaveImageAsync(model.FeaturedImageFile, uploadFolder);
                post.FeaturedImage = $"/uploads/blog/{fileName}";
            }

            await _blogService.UpdatePostAsync(post);

            TempData["Success"] = "Blog yazısı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating blog post");
            ModelState.AddModelError("", "Blog yazısı güncellenirken bir hata oluştu.");

            ViewBag.Categories = _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToList();

            return View(model);
        }
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post != null)
            {
                // Delete image if exists
                if (!string.IsNullOrEmpty(post.FeaturedImage))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, post.FeaturedImage.TrimStart('/'));
                    ImageHelper.DeleteImage(imagePath);
                }

                await _blogService.DeletePostAsync(id);
                TempData["Success"] = "Blog yazısı başarıyla silindi.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blog post");
            TempData["Error"] = "Blog yazısı silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }

    // Document Management

    [HttpPost("upload-document/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDocument(int id, IFormFile documentFile, string? displayName, string? description)
    {
        try
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                TempData["Error"] = "Blog yazısı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (documentFile == null || !DocumentHelper.IsValidDocument(documentFile))
            {
                TempData["Error"] = "Geçersiz dosya. İzin verilen formatlar: PDF, Word, Excel, PowerPoint, ZIP (Max 10MB)";
                return RedirectToAction(nameof(Edit), new { id });
            }

            var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "documents");
            var (fileName, filePath) = await DocumentHelper.SaveDocumentAsync(documentFile, uploadFolder);

            var document = new BlogPostDocument
            {
                BlogPostId = id,
                FileName = fileName,
                FilePath = $"/uploads/documents/{fileName}",
                DisplayName = displayName ?? documentFile.FileName,
                Description = description,
                FileSize = documentFile.Length,
                FileType = Path.GetExtension(documentFile.FileName).ToLowerInvariant(),
                UploadedAt = DateTime.UtcNow
            };

            _context.BlogPostDocuments.Add(document);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Evrak başarıyla yüklendi.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");
            TempData["Error"] = "Evrak yüklenirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost("delete-document/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(int id, int blogPostId)
    {
        try
        {
            var document = await _context.BlogPostDocuments.FindAsync(id);
            if (document != null)
            {
                // Delete physical file
                var fullPath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                DocumentHelper.DeleteDocument(fullPath);

                _context.BlogPostDocuments.Remove(document);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Evrak başarıyla silindi.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document");
            TempData["Error"] = "Evrak silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Edit), new { id = blogPostId });
    }

    [HttpGet("download-document/{id}")]
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var document = await _context.BlogPostDocuments.FindAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        // Increment download count
        document.DownloadCount++;
        await _context.SaveChangesAsync();

        var fullPath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound();
        }

        var memory = new MemoryStream();
        using (var stream = new FileStream(fullPath, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;

        var contentType = document.FileType switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            _ => "application/octet-stream"
        };

        return File(memory, contentType, document.DisplayName ?? document.FileName);
    }
}
