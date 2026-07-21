using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace backend.Helpers;

public static class SlugHelper
{
    public static string Generate(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        string normalizedText = text.Normalize(NormalizationForm.FormD);

        StringBuilder slug = new();

        foreach (char character in normalizedText)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(character);

            if (category != UnicodeCategory.NonSpacingMark)
            {
                slug.Append(character);
            }
        }

        string result = slug
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant();

        result = Regex.Replace(result, @"[^a-z0-9\s-]", "");
        result = Regex.Replace(result, @"\s+", "-");
        result = Regex.Replace(result, @"-+", "-");

        return result.Trim('-');
    }
}