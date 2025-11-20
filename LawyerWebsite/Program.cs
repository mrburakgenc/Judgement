using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using LawyerWebsite.Data;
using LawyerWebsite.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllersWithViews();

// Database Context - PostgreSQL
// Parse Railway DATABASE_URL if available
string connectionString;
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    // Railway format: postgresql://user:password@host:port/database
    var uri = new Uri(databaseUrl);
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        // SameAsRequest for Railway (runs behind reverse proxy)
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
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

// Response Caching
builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Don't use HSTS in production (Railway handles HTTPS)
}
else
{
    app.UseDeveloperExceptionPage();
}

// Don't redirect to HTTPS in production (Railway handles this)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

// Map controllers with attribute routing
app.MapControllers();

// Blog detail route (for SEO friendly URLs)
app.MapControllerRoute(
    name: "blogDetail",
    pattern: "blog/{slug}",
    defaults: new { controller = "Blog", action = "Detail" });

// Default route (public pages only)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

try
{
    Log.Information("Starting web application");

    // Migrate and seed database
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Auto migrate on production
        Log.Information("Applying database migrations...");
        await context.Database.MigrateAsync();

        // Seed database
        Log.Information("Seeding database...");
        await DbInitializer.SeedAsync(context);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
