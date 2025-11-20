namespace LawyerWebsite.Helpers;

public static class ImageHelper
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public static bool IsValidImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        // Check file size
        if (file.Length > MaxFileSize)
            return false;

        // Check file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return false;

        return true;
    }

    public static async Task<string> SaveImageAsync(IFormFile file, string uploadFolder)
    {
        if (!IsValidImage(file))
            throw new ArgumentException("Invalid image file");

        // Create upload directory if it doesn't exist
        Directory.CreateDirectory(uploadFolder);

        // Generate unique filename
        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadFolder, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueFileName;
    }

    public static void DeleteImage(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            File.Delete(imagePath);
        }
    }
}
