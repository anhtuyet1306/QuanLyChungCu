using System.Globalization;
using System.Text;

namespace QuanLyChungCu_SkyResidence.Helpers;

public static class TextHelper
{
    /// <summary>Bỏ dấu tiếng Việt: "Nguyễn Văn Đức" -> "Nguyen Van Duc".</summary>
    public static string RemoveAccents(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        string normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);
        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC)
                 .Replace('đ', 'd').Replace('Đ', 'D');
    }

    /// <summary>Tìm kiếm không phân biệt hoa thường và dấu (gõ "nguyen" vẫn ra "Nguyễn").</summary>
    public static bool ContainsIgnoreAccent(string? source, string keyword)
    {
        return RemoveAccents(source).Contains(
            RemoveAccents(keyword).Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
