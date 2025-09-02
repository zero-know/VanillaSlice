namespace VanillaSlice.Bootstrapper.Services;

/// <summary>
/// Service for converting singular nouns to their plural forms using English pluralization rules
/// </summary>
public class PluralizationService
{
    private readonly Dictionary<string, string> _irregularPlurals = new()
    {
        { "person", "people" },
        { "child", "children" },
        { "foot", "feet" },
        { "tooth", "teeth" },
        { "goose", "geese" },
        { "mouse", "mice" },
        { "man", "men" },
        { "woman", "women" },
        { "ox", "oxen" },
        { "sheep", "sheep" },
        { "deer", "deer" },
        { "fish", "fish" },
        { "species", "species" },
        { "series", "series" }
    };

    private readonly HashSet<string> _uncountableNouns = new()
    {
        "information", "water", "rice", "money", "music", "advice", "equipment",
        "furniture", "homework", "knowledge", "luggage", "news", "progress",
        "research", "software", "traffic", "weather", "work"
    };

    /// <summary>
    /// Converts a singular noun to its plural form
    /// </summary>
    /// <param name="singular">The singular noun to pluralize</param>
    /// <returns>The plural form of the noun</returns>
    public string Pluralize(string singular)
    {
        if (string.IsNullOrWhiteSpace(singular))
            return singular;

        var lowerSingular = singular.ToLowerInvariant();

        // Check for uncountable nouns
        if (_uncountableNouns.Contains(lowerSingular))
            return singular;

        // Check for irregular plurals
        if (_irregularPlurals.TryGetValue(lowerSingular, out var irregularPlural))
        {
            return PreserveCasing(singular, irregularPlural);
        }

        // Apply standard pluralization rules
        return ApplyStandardRules(singular);
    }

    private string ApplyStandardRules(string singular)
    {
        var lower = singular.ToLowerInvariant();

        // Words ending in 'y' preceded by a consonant: change 'y' to 'ies'
        if (lower.EndsWith("y") && singular.Length > 1 && !IsVowel(lower[^2]))
        {
            return singular[..^1] + "ies";
        }

        // Words ending in 's', 'ss', 'sh', 'ch', 'x', 'z': add 'es'
        if (lower.EndsWith("s") || lower.EndsWith("ss") || lower.EndsWith("sh") || 
            lower.EndsWith("ch") || lower.EndsWith("x") || lower.EndsWith("z"))
        {
            return singular + "es";
        }

        // Words ending in 'f' or 'fe': change to 'ves'
        if (lower.EndsWith("f"))
        {
            return singular[..^1] + "ves";
        }
        if (lower.EndsWith("fe"))
        {
            return singular[..^2] + "ves";
        }

        // Words ending in 'o' preceded by a consonant: add 'es'
        if (lower.EndsWith("o") && singular.Length > 1 && !IsVowel(lower[^2]))
        {
            // Some exceptions that just add 's'
            var oExceptions = new[] { "photo", "piano", "halo", "auto", "memo", "pro", "solo" };
            if (!oExceptions.Contains(lower))
            {
                return singular + "es";
            }
        }

        // Default rule: add 's'
        return singular + "s";
    }

    private bool IsVowel(char c)
    {
        return "aeiou".Contains(char.ToLowerInvariant(c));
    }

    private string PreserveCasing(string original, string replacement)
    {
        if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(replacement))
            return replacement;

        // If original is all uppercase, make replacement all uppercase
        if (original.All(char.IsUpper))
            return replacement.ToUpperInvariant();

        // If original starts with uppercase, capitalize first letter of replacement
        if (char.IsUpper(original[0]))
            return char.ToUpperInvariant(replacement[0]) + replacement[1..].ToLowerInvariant();

        // Otherwise, keep replacement as lowercase
        return replacement.ToLowerInvariant();
    }

    /// <summary>
    /// Converts a plural noun back to its singular form (basic implementation)
    /// </summary>
    /// <param name="plural">The plural noun to singularize</param>
    /// <returns>The singular form of the noun</returns>
    public string Singularize(string plural)
    {
        if (string.IsNullOrWhiteSpace(plural))
            return plural;

        var lower = plural.ToLowerInvariant();

        // Check reverse irregular plurals
        var reverseIrregular = _irregularPlurals.FirstOrDefault(kvp => kvp.Value.ToLowerInvariant() == lower);
        if (!reverseIrregular.Equals(default(KeyValuePair<string, string>)))
        {
            return PreserveCasing(plural, reverseIrregular.Key);
        }

        // Basic singularization rules (reverse of pluralization)
        if (lower.EndsWith("ies") && plural.Length > 3)
        {
            return plural[..^3] + "y";
        }

        if (lower.EndsWith("ves") && plural.Length > 3)
        {
            return plural[..^3] + "f";
        }

        if (lower.EndsWith("es") && plural.Length > 2)
        {
            var withoutEs = plural[..^2];
            var lowerWithoutEs = withoutEs.ToLowerInvariant();
            if (lowerWithoutEs.EndsWith("s") || lowerWithoutEs.EndsWith("sh") || 
                lowerWithoutEs.EndsWith("ch") || lowerWithoutEs.EndsWith("x") || 
                lowerWithoutEs.EndsWith("z"))
            {
                return withoutEs;
            }
        }

        if (lower.EndsWith("s") && plural.Length > 1)
        {
            return plural[..^1];
        }

        return plural;
    }
}
