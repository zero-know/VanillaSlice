using System.Text.Json.Serialization;

namespace {{RootNamespace}}.SliceFactory.Cli;

/// <summary>
/// Represents the slices manifest file that tracks all generated slices
/// </summary>
public class SliceManifest
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("slices")]
    public List<SliceDefinition> Slices { get; set; } = new();
}

/// <summary>
/// Represents a single slice definition in the manifest
/// </summary>
public class SliceDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("componentPrefix")]
    public string ComponentPrefix { get; set; } = string.Empty;

    [JsonPropertyName("featurePluralName")]
    public string FeaturePluralName { get; set; } = string.Empty;

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = string.Empty;

    [JsonPropertyName("directoryName")]
    public string DirectoryName { get; set; } = string.Empty;

    [JsonPropertyName("primaryKeyType")]
    public string PrimaryKeyType { get; set; } = "Guid";

    [JsonPropertyName("generateForm")]
    public bool GenerateForm { get; set; } = true;

    [JsonPropertyName("generateListing")]
    public bool GenerateListing { get; set; } = true;

    [JsonPropertyName("generateSelectList")]
    public bool GenerateSelectList { get; set; } = false;

    [JsonPropertyName("selectListModelType")]
    public string SelectListModelType { get; set; } = "SelectOption";

    [JsonPropertyName("selectListDataType")]
    public string SelectListDataType { get; set; } = "string";

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("lastGeneratedAt")]
    public DateTime LastGeneratedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("generatedFiles")]
    public List<string> GeneratedFiles { get; set; } = new();

    /// <summary>
    /// Generates a unique ID for this slice based on namespace and component prefix
    /// </summary>
    public static string GenerateId(string ns, string componentPrefix)
    {
        return $"{ns.ToLowerInvariant()}-{componentPrefix.ToLowerInvariant()}".Replace(".", "-");
    }
}
