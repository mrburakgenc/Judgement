using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LawyerWebsite.Helpers;

public static class SlugHelper
{
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Replace Turkish characters
        text = text
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c")
            .Replace("İ", "i")
            .Replace("Ğ", "g")
            .Replace("Ü", "u")
            .Replace("Ş", "s")
            .Replace("Ö", "o")
            .Replace("Ç", "c");

        // Remove accents
        text = RemoveAccents(text);

        // Remove invalid characters
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

        // Convert multiple spaces into one space
        text = Regex.Replace(text, @"\s+", " ").Trim();

        // Replace spaces with hyphens
        text = Regex.Replace(text, @"\s", "-");

        // Remove multiple hyphens
        text = Regex.Replace(text, @"-+", "-");

        return text;
    }

    private static string RemoveAccents(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        text = text.Normalize(NormalizationForm.FormD);
        var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
        return new string(chars).Normalize(NormalizationForm.FormC);
    }
}
