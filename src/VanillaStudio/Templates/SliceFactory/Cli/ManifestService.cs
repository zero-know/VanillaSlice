using System.Text.Json;

namespace {{RootNamespace}}.SliceFactory.Cli;

/// <summary>
/// Service for managing the slices manifest file
/// </summary>
public class ManifestService
{
    private readonly string _manifestPath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ManifestService(string? basePath = null)
    {
        // Default to solution root or current directory
        var solutionRoot = basePath ?? FindSolutionRoot() ?? Directory.GetCurrentDirectory();
        _manifestPath = Path.Combine(solutionRoot, "slices-manifest.json");
    }

    /// <summary>
    /// Gets the manifest file path
    /// </summary>
    public string ManifestPath => _manifestPath;

    /// <summary>
    /// Load the manifest from disk, or create a new one if it doesn't exist
    /// </summary>
    public async Task<SliceManifest> LoadAsync()
    {
        if (!File.Exists(_manifestPath))
        {
            return new SliceManifest();
        }

        try
        {
            var json = await File.ReadAllTextAsync(_manifestPath);
            return JsonSerializer.Deserialize<SliceManifest>(json, JsonOptions) ?? new SliceManifest();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not parse manifest file: {ex.Message}");
            return new SliceManifest();
        }
    }

    /// <summary>
    /// Save the manifest to disk
    /// </summary>
    public async Task SaveAsync(SliceManifest manifest)
    {
        manifest.LastUpdated = DateTime.UtcNow;
        var json = JsonSerializer.Serialize(manifest, JsonOptions);
        await File.WriteAllTextAsync(_manifestPath, json);
    }

    /// <summary>
    /// Add or update a slice in the manifest
    /// </summary>
    public async Task<SliceDefinition> AddOrUpdateSliceAsync(SliceDefinition slice, List<string>? generatedFiles = null)
    {
        var manifest = await LoadAsync();

        // Generate ID if not set
        if (string.IsNullOrEmpty(slice.Id))
        {
            slice.Id = SliceDefinition.GenerateId(slice.Namespace, slice.ComponentPrefix);
        }

        // Find existing slice or add new
        var existingIndex = manifest.Slices.FindIndex(s => s.Id == slice.Id);
        if (existingIndex >= 0)
        {
            // Update existing
            slice.CreatedAt = manifest.Slices[existingIndex].CreatedAt;
            slice.LastGeneratedAt = DateTime.UtcNow;
            if (generatedFiles != null)
            {
                slice.GeneratedFiles = generatedFiles;
            }
            manifest.Slices[existingIndex] = slice;
        }
        else
        {
            // Add new
            slice.CreatedAt = DateTime.UtcNow;
            slice.LastGeneratedAt = DateTime.UtcNow;
            if (generatedFiles != null)
            {
                slice.GeneratedFiles = generatedFiles;
            }
            manifest.Slices.Add(slice);
        }

        await SaveAsync(manifest);
        return slice;
    }

    /// <summary>
    /// Get a slice by ID
    /// </summary>
    public async Task<SliceDefinition?> GetSliceAsync(string id)
    {
        var manifest = await LoadAsync();
        return manifest.Slices.FirstOrDefault(s =>
            s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Get all slices
    /// </summary>
    public async Task<List<SliceDefinition>> GetAllSlicesAsync()
    {
        var manifest = await LoadAsync();
        return manifest.Slices;
    }

    /// <summary>
    /// Remove a slice from the manifest
    /// </summary>
    public async Task<bool> RemoveSliceAsync(string id)
    {
        var manifest = await LoadAsync();
        var removed = manifest.Slices.RemoveAll(s =>
            s.Id.Equals(id, StringComparison.OrdinalIgnoreCase)) > 0;

        if (removed)
        {
            await SaveAsync(manifest);
        }

        return removed;
    }

    /// <summary>
    /// Create a SliceDefinition from CLI options
    /// </summary>
    public static SliceDefinition FromCliOptions(CliOptions options)
    {
        return new SliceDefinition
        {
            Id = SliceDefinition.GenerateId(options.Namespace!, options.ComponentPrefix!),
            ComponentPrefix = options.ComponentPrefix!,
            FeaturePluralName = options.FeaturePluralName!,
            Namespace = options.Namespace!,
            DirectoryName = options.DirectoryName!,
            PrimaryKeyType = options.PrimaryKeyType,
            GenerateForm = options.GenerateForm,
            GenerateListing = options.GenerateListing,
            GenerateSelectList = options.GenerateSelectList,
            SelectListModelType = options.SelectListModelType,
            SelectListDataType = options.SelectListDataType
        };
    }

    /// <summary>
    /// Find the solution root by looking for .sln files
    /// </summary>
    private static string? FindSolutionRoot()
    {
        var current = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(current);

        while (directory != null)
        {
            if (directory.GetFiles("*.sln").Length > 0)
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }

        return null;
    }
}
