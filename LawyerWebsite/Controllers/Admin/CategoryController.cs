using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerWebsite.Data;
using LawyerWebsite.Models.Entities;
using LawyerWebsite.Helpers;

namespace LawyerWebsite.Controllers.Admin;

[Authorize]
[Route("admin/category")]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return View(categories);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model)
    {
        // Slug otomatik oluşturulacağı için ModelState'den kaldır
        ModelState.Remove("Slug");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            model.Slug = SlugHelper.GenerateSlug(model.Name);
            model.CreatedAt = DateTime.UtcNow;

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kategori başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            ModelState.AddModelError("", "Kategori oluşturulurken bir hata oluştu.");
            return View(model);
        }
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        // Slug otomatik oluşturulacağı için ModelState'den kaldır
        ModelState.Remove("Slug");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = model.Name;
            category.Slug = SlugHelper.GenerateSlug(model.Name);
            category.Description = model.Description;
            category.DisplayOrder = model.DisplayOrder;
            category.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Kategori başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            ModelState.AddModelError("", "Kategori güncellenirken bir hata oluştu.");
            return View(model);
        }
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategori başarıyla silindi.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            TempData["Error"] = "Kategori silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }
}
