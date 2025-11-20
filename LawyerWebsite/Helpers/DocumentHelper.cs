namespace LawyerWebsite.Helpers;

public static class DocumentHelper
{
    private static readonly string[] AllowedExtensions =
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".zip", ".rar"
    };

    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public static bool IsValidDocument(IFormFile file)
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

    public static async Task<(string fileName, string filePath)> SaveDocumentAsync(IFormFile file, string uploadFolder)
    {
        if (!IsValidDocument(file))
            throw new ArgumentException("Invalid document file");

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

        return (uniqueFileName, filePath);
    }

    public static void DeleteDocument(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public static string GetFileIcon(string fileType)
    {
        return fileType.ToLowerInvariant() switch
        {
            ".pdf" => "bi-file-earmark-pdf",
            ".doc" or ".docx" => "bi-file-earmark-word",
            ".xls" or ".xlsx" => "bi-file-earmark-excel",
            ".ppt" or ".pptx" => "bi-file-earmark-slides",
            ".zip" or ".rar" => "bi-file-earmark-zip",
            ".txt" => "bi-file-earmark-text",
            _ => "bi-file-earmark"
        };
    }

    public static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}
