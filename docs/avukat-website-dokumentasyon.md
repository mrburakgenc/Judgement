# Avukat Tanıtım ve Blog Sitesi - Yazılım Dokümantasyonu

## 1. Proje Genel Bakış

### 1.1 Proje Tanımı
Avukat için kişisel tanıtım sitesi ve blog platformu. Monolithic mimari ile geliştirilecek, admin panelinden içerik yönetimi sağlanacak.

### 1.2 Temel Özellikler
- **Tanıtım Sayfası**: Avukat hakkında bilgi, uzmanlık alanları, iletişim
- **Blog Sistemi**: Yazı ekleme, düzenleme, silme, kategorilendirme
- **İletişim Formu**: Ziyaretçilerden gelen mesajların mail olarak iletilmesi
- **Admin Paneli**: İçerik yönetimi için basit yönetim arayüzü

## 2. Teknoloji Stack

### 2.1 Backend & Frontend (Monolithic)
```
- ASP.NET Core 8.0 MVC
- Razor Pages / Views
- Entity Framework Core 8.0
- SQL Server / PostgreSQL
- Bootstrap 5 (UI Framework)
```

### 2.2 Kütüphaneler
```
- MailKit (Mail gönderimi)
- TinyMCE / CKEditor (Zengin metin editörü)
- AutoMapper (DTO mapping)
- FluentValidation (Validasyon)
- Serilog (Loglama)
```

## 3. Veritabanı Şeması

### 3.1 Tablolar

```sql
-- Admin Kullanıcı Tablosu
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE()
)

-- Blog Kategorileri
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Slug NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    DisplayOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE()
)

-- Blog Yazıları
CREATE TABLE BlogPosts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Slug NVARCHAR(200) NOT NULL UNIQUE,
    Summary NVARCHAR(500),
    Content NVARCHAR(MAX) NOT NULL,
    CategoryId INT,
    FeaturedImage NVARCHAR(500),
    MetaTitle NVARCHAR(200),
    MetaDescription NVARCHAR(500),
    MetaKeywords NVARCHAR(300),
    ViewCount INT DEFAULT 0,
    IsPublished BIT DEFAULT 0,
    PublishedAt DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE SET NULL
)

-- İletişim Mesajları
CREATE TABLE ContactMessages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Subject NVARCHAR(200),
    Message NVARCHAR(2000) NOT NULL,
    IsRead BIT DEFAULT 0,
    IsReplied BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE()
)

-- Site Ayarları
CREATE TABLE SiteSettings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SettingKey NVARCHAR(100) NOT NULL UNIQUE,
    SettingValue NVARCHAR(MAX),
    Description NVARCHAR(500)
)
```

## 4. Proje Yapısı

```
LawyerWebsite/
│
├── Controllers/
│   ├── HomeController.cs              # Ana sayfa
│   ├── BlogController.cs              # Blog listeleme, detay
│   ├── ContactController.cs           # İletişim formu
│   └── Admin/
│       ├── AdminController.cs         # Admin dashboard
│       ├── BlogManagementController.cs
│       ├── CategoryController.cs
│       └── MessagesController.cs
│
├── Models/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Category.cs
│   │   ├── BlogPost.cs
│   │   ├── ContactMessage.cs
│   │   └── SiteSetting.cs
│   │
│   ├── ViewModels/
│   │   ├── BlogPostViewModel.cs
│   │   ├── ContactFormViewModel.cs
│   │   └── Admin/
│   │       ├── BlogPostCreateViewModel.cs
│   │       ├── BlogPostEditViewModel.cs
│   │       └── DashboardViewModel.cs
│   │
│   └── DTOs/
│       └── EmailDto.cs
│
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/
│
├── Services/
│   ├── IEmailService.cs
│   ├── EmailService.cs
│   ├── IBlogService.cs
│   ├── BlogService.cs
│   └── SlugService.cs
│
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _AdminLayout.cshtml
│   │   └── Components/
│   │
│   ├── Home/
│   │   ├── Index.cshtml              # Ana sayfa
│   │   └── About.cshtml              # Hakkımda
│   │
│   ├── Blog/
│   │   ├── Index.cshtml              # Blog listesi
│   │   └── Detail.cshtml             # Blog detay
│   │
│   ├── Contact/
│   │   └── Index.cshtml
│   │
│   └── Admin/
│       ├── Dashboard/Index.cshtml
│       ├── Blog/
│       │   ├── Index.cshtml
│       │   ├── Create.cshtml
│       │   └── Edit.cshtml
│       └── Messages/Index.cshtml
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── images/
│   └── uploads/
│       └── blog/
│
├── Helpers/
│   ├── ImageHelper.cs
│   └── SlugHelper.cs
│
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
│
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

## 5. Temel Entity Sınıfları

### 5.1 BlogPost Entity
```csharp
public class BlogPost
{
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public string Title { get; set; }
    
    [Required, MaxLength(200)]
    public string Slug { get; set; }
    
    [MaxLength(500)]
    public string Summary { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    public int? CategoryId { get; set; }
    public Category Category { get; set; }
    
    [MaxLength(500)]
    public string FeaturedImage { get; set; }
    
    public int ViewCount { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // SEO
    [MaxLength(200)]
    public string MetaTitle { get; set; }
    [MaxLength(500)]
    public string MetaDescription { get; set; }
    [MaxLength(300)]
    public string MetaKeywords { get; set; }
}
```

### 5.2 ContactMessage Entity
```csharp
public class ContactMessage
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string FullName { get; set; }
    
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; }
    
    [Phone, MaxLength(20)]
    public string Phone { get; set; }
    
    [MaxLength(200)]
    public string Subject { get; set; }
    
    [Required, MaxLength(2000)]
    public string Message { get; set; }
    
    public bool IsRead { get; set; }
    public bool IsReplied { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## 6. Servis Katmanı Örnekleri

### 6.1 Email Service Interface
```csharp
public interface IEmailService
{
    Task SendContactFormEmailAsync(ContactMessage message);
    Task SendEmailAsync(string to, string subject, string body);
}
```

### 6.2 Email Service Implementation
```csharp
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendContactFormEmailAsync(ContactMessage message)
    {
        var emailBody = $@"
            <h2>Yeni İletişim Formu Mesajı</h2>
            <p><strong>Ad Soyad:</strong> {message.FullName}</p>
            <p><strong>E-posta:</strong> {message.Email}</p>
            <p><strong>Telefon:</strong> {message.Phone}</p>
            <p><strong>Konu:</strong> {message.Subject}</p>
            <p><strong>Mesaj:</strong></p>
            <p>{message.Message}</p>
            <p><small>Gönderim Tarihi: {message.CreatedAt:dd.MM.yyyy HH:mm}</small></p>
        ";
        
        var lawyerEmail = _configuration["EmailSettings:LawyerEmail"];
        await SendEmailAsync(lawyerEmail, 
            $"Yeni İletişim Formu: {message.Subject}", 
            emailBody);
    }
    
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(
            _configuration["EmailSettings:SmtpServer"],
            int.Parse(_configuration["EmailSettings:SmtpPort"]),
            SecureSocketOptions.StartTls
        );
        
        await client.AuthenticateAsync(
            _configuration["EmailSettings:Username"],
            _configuration["EmailSettings:Password"]
        );
        
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["EmailSettings:SenderName"],
            _configuration["EmailSettings:SenderEmail"]
        ));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = body };
        
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
```

### 6.3 Blog Service
```csharp
public interface IBlogService
{
    Task<List<BlogPost>> GetPublishedPostsAsync(int? categoryId = null);
    Task<BlogPost> GetPostBySlugAsync(string slug);
    Task<BlogPost> CreatePostAsync(BlogPost post);
    Task UpdatePostAsync(BlogPost post);
    Task DeletePostAsync(int id);
    Task IncrementViewCountAsync(int postId);
}
```

## 7. Controller Örnekleri

### 7.1 Home Controller
```csharp
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public IActionResult Index()
    {
        var recentPosts = _context.BlogPosts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .Take(3)
            .Include(p => p.Category)
            .ToList();
            
        return View(recentPosts);
    }
    
    public IActionResult About()
    {
        return View();
    }
}
```

### 7.2 Blog Controller
```csharp
public class BlogController : Controller
{
    private readonly IBlogService _blogService;
    
    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }
    
    public async Task<IActionResult> Index(int? categoryId, int page = 1)
    {
        var posts = await _blogService.GetPublishedPostsAsync(categoryId);
        
        // Pagination logic
        int pageSize = 10;
        var pagedPosts = posts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        ViewBag.TotalPages = (int)Math.Ceiling(posts.Count / (double)pageSize);
        ViewBag.CurrentPage = page;
        
        return View(pagedPosts);
    }
    
    public async Task<IActionResult> Detail(string slug)
    {
        var post = await _blogService.GetPostBySlugAsync(slug);
        
        if (post == null)
            return NotFound();
            
        await _blogService.IncrementViewCountAsync(post.Id);
        
        return View(post);
    }
}
```

### 7.3 Contact Controller
```csharp
public class ContactController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    
    public ContactController(
        ApplicationDbContext context, 
        IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
            
        var message = new ContactMessage
        {
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            Subject = model.Subject,
            Message = model.Message,
            CreatedAt = DateTime.Now
        };
        
        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync();
        
        // Email gönder
        try
        {
            await _emailService.SendContactFormEmailAsync(message);
        }
        catch (Exception ex)
        {
            // Log error but don't fail
            // Kullanıcıya mesajın alındığını göster
        }
        
        TempData["Success"] = "Mesajınız başarıyla gönderildi. En kısa sürede dönüş yapılacaktır.";
        return RedirectToAction(nameof(Index));
    }
}
```

### 7.4 Admin Blog Management Controller
```csharp
[Authorize] // Authentication gerekli
[Route("admin/blog")]
public class BlogManagementController : Controller
{
    private readonly IBlogService _blogService;
    private readonly ApplicationDbContext _context;
    
    [HttpGet]
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
                .ToList();
            return View(model);
        }
        
        var post = new BlogPost
        {
            Title = model.Title,
            Slug = SlugHelper.GenerateSlug(model.Title),
            Summary = model.Summary,
            Content = model.Content,
            CategoryId = model.CategoryId,
            IsPublished = model.IsPublished,
            PublishedAt = model.IsPublished ? DateTime.Now : null,
            CreatedAt = DateTime.Now,
            MetaTitle = model.MetaTitle,
            MetaDescription = model.MetaDescription,
            MetaKeywords = model.MetaKeywords
        };
        
        // Resim yükleme
        if (model.FeaturedImageFile != null)
        {
            var fileName = await SaveImageAsync(model.FeaturedImageFile);
            post.FeaturedImage = fileName;
        }
        
        await _blogService.CreatePostAsync(post);
        
        TempData["Success"] = "Blog yazısı başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }
    
    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine("wwwroot", "uploads", "blog");
        Directory.CreateDirectory(uploadsFolder);
        
        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        
        return $"/uploads/blog/{uniqueFileName}";
    }
}
```

## 8. Yapılandırma (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LawyerWebsite;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "SenderName": "Avukat Web Sitesi",
    "SenderEmail": "noreply@avukatsite.com",
    "LawyerEmail": "avukat@example.com"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 9. Program.cs Yapılandırması

```csharp
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBlogService, BlogService>();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "blog",
    pattern: "blog/{slug}",
    defaults: new { controller = "Blog", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

## 10. Güvenlik Önlemleri

### 10.1 Authentication & Authorization
- Cookie-based authentication
- Admin paneli için `[Authorize]` attribute
- Password hashing (ASP.NET Identity veya BCrypt)
- CSRF token koruması (Anti-forgery tokens)

### 10.2 Input Validation
- FluentValidation kullanımı
- XSS koruması (HTML encoding)
- SQL Injection koruması (EF Core parametreli sorgular)
- File upload validation (boyut, tip kontrolü)

### 10.3 Diğer
- HTTPS zorunlu
- Rate limiting (contact form için)
- Error handling middleware
- Logging (Serilog)

## 11. Deployment Checklist

### 11.1 Üretim Öncesi
- [ ] Connection string güncelle
- [ ] Email ayarlarını yapılandır
- [ ] HTTPS sertifikası yükle
- [ ] Veritabanı migration'ları çalıştır
- [ ] Static files compression aktif et
- [ ] Response caching ekle
- [ ] Sitemap.xml oluştur
- [ ] robots.txt ekle

### 11.2 SEO
- [ ] Meta tags tüm sayfalarda
- [ ] Open Graph tags
- [ ] Schema.org markup (avukat için)
- [ ] XML sitemap
- [ ] Google Analytics entegrasyonu
- [ ] Google Search Console kayıt

## 12. İlk Kurulum Adımları

```bash
# 1. Proje oluştur
dotnet new mvc -n LawyerWebsite

# 2. Gerekli paketleri yükle
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package MailKit
dotnet add package FluentValidation.AspNetCore
dotnet add package Serilog.AspNetCore

# 3. Migration oluştur ve çalıştır
dotnet ef migrations add InitialCreate
dotnet ef database update

# 4. İlk admin kullanıcısını oluştur (Seed data)
# 5. Projeyi çalıştır
dotnet run
```

## 13. Ek Özellikler (Opsiyonel)

### 13.1 Önbellek (Cache)
- Response caching
- Distributed cache (Redis)
- Memory cache for frequent queries

### 13.2 Performans
- Image optimization (compression)
- Lazy loading
- CDN entegrasyonu
- Minification (CSS, JS)

### 13.3 SEO Enhancements
- Breadcrumb navigation
- Related posts
- Social media sharing buttons
- Comment system (optional)

### 13.4 Analytics & Monitoring
- Google Analytics
- Application Insights
- Error tracking (Sentry)
- Performance monitoring

## 14. Bakım & Güncelleme

### 14.1 Düzenli Bakım
- Veritabanı yedekleme
- Log dosyası temizleme
- Security updates
- Dependency updates

### 14.2 Monitoring
- Server health checks
- Database performance
- Email delivery status
- Error logs review

---

## Notlar

- **TinyMCE**: Blog yazıları için zengin metin editörü. CDN üzerinden kullanılabilir.
- **Resim Yönetimi**: Alternatif olarak Cloudinary veya AWS S3 kullanılabilir.
- **Email Template**: Razor view'lar ile HTML email template'leri oluşturulabilir.
- **Sitemap**: Otomatik sitemap generator eklenebilir.
- **Cache**: Yoğun trafikte Redis cache önerilir.

Bu dokümantasyon minimal ama fonksiyonel bir yapı sunmaktadır. İhtiyaçlara göre genişletilebilir ve özelleştirilebilir!
