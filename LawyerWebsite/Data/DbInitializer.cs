using LawyerWebsite.Models.Entities;
using LawyerWebsite.Controllers.Admin;

namespace LawyerWebsite.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Create database if not exists
        await context.Database.EnsureCreatedAsync();

        // Check if admin user exists
        if (context.Users.Any())
        {
            return; // Database has been seeded
        }

        // Create default admin user
        var adminUser = new User
        {
            Email = "admin@lawyer.com",
            PasswordHash = AuthController.CreatePasswordHash("Admin123!"),
            FullName = "Admin User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);

        // Create default categories
        var categories = new[]
        {
            new Category
            {
                Name = "Aile Hukuku",
                Slug = "aile-hukuku",
                Description = "Boşanma, velayet ve nafaka davaları",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Ticaret Hukuku",
                Slug = "ticaret-hukuku",
                Description = "Şirket kuruluşu, ticari sözleşmeler",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Ceza Hukuku",
                Slug = "ceza-hukuku",
                Description = "Ceza davaları ve savunma",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "İş Hukuku",
                Slug = "is-hukuku",
                Description = "İşe iade, kıdem tazminatı davaları",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Categories.AddRange(categories);

        // Create sample site settings
        var settings = new[]
        {
            new SiteSetting
            {
                SettingKey = "SiteName",
                SettingValue = "Av. [İsim Soyisim]",
                Description = "Web sitesi adı"
            },
            new SiteSetting
            {
                SettingKey = "SiteDescription",
                SettingValue = "Profesyonel hukuki danışmanlık hizmetleri",
                Description = "Site açıklaması"
            },
            new SiteSetting
            {
                SettingKey = "ContactPhone",
                SettingValue = "+90 XXX XXX XX XX",
                Description = "İletişim telefonu"
            },
            new SiteSetting
            {
                SettingKey = "ContactEmail",
                SettingValue = "info@example.com",
                Description = "İletişim e-posta"
            },
            new SiteSetting
            {
                SettingKey = "OfficeAddress",
                SettingValue = "Adres bilgisi buraya gelecek",
                Description = "Ofis adresi"
            }
        };

        context.SiteSettings.AddRange(settings);

        await context.SaveChangesAsync();
    }
}
