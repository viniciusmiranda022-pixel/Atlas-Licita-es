using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace AtlasLicitacoes.Services;

public sealed class SearchClassifier
{
    private readonly List<string> _keywords;

    public SearchClassifier(IOptions<SearchRuleOptions> options)
    {
        _keywords = options.Value.Keywords
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(Normalize)
            .Distinct()
            .ToList();
    }

    public List<string> GetMatches(params string?[] fields)
    {
        var text = Normalize(string.Join(' ', fields.Where(x => !string.IsNullOrWhiteSpace(x))));
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        return _keywords.Where(text.Contains).ToList();
    }

    private static string Normalize(string input)
    {
        var lower = input.Trim().ToLowerInvariant();
        var normalized = lower.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
