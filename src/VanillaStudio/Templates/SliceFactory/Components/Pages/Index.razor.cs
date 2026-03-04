using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using {{RootNamespace}}.SliceFactory.Services;
using {{RootNamespace}}.SliceFactory.Models;

namespace {{RootNamespace}}.SliceFactory.Components.Pages;


public partial class Index
{
    [Inject] private TemplateEngineService TemplateEngine { get; set; } = default!;
    [Inject] private FeatureManagementService FeatureService { get; set; } = default!;
    [Inject] private PlacementGuidanceService PlacementService { get; set; } = default!;
    [Inject] private PathDetectionService PathDetectionService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private bool HasValidationErrors => !string.IsNullOrEmpty(M.DirectoryName) &&
                                       !string.IsNullOrEmpty(M.NameSpace) &&
                                       !string.IsNullOrEmpty(M.PkType);

    const string __projectNamespace__ = "__projectNamespace__";
    const string __componentPrefix__ = "__ComponentPrefix__";
    const string __moduleNamespace__ = "__moduleNamespace__";
    const string __frameworkNamespace__ = "__frameworkNamespace__";
    const string __dbContextNamespace__ = "__dbContextNamespace__";
    const string __primaryKeyType__ = "__primaryKeyType__";
    const string __primaryKeyTypeClientResponse__ = "__primaryKeyTypeClientResponse__";

    public FormViewModel M { get; set; } = new FormViewModel()
    {
        PkType = string.Empty,
        GenerateForm = true,
        GenerateListing = true,
        GenerateSelectList = false,
        SelectListModelType = "SelectOption",
        SelectListDataType = "string",
        GenerateControllerAndClientService = true
    };

    // Preview functionality
    private bool ShowFilePreview = true;
    private bool IsPreviewLoading = false;
    private List<FeatureFilePreview>? PreviewFiles;
    private List<FeatureFilePreview> _existingPreviews = new();
    private CancellationTokenSource? _previewDebounceTokenSource;

    // Placement guidance functionality
    private PlacementGuidance? PlacementGuidance;
    private bool ShowPlacementGuidance = false;

    // Success message functionality
    private bool ShowSuccessMessage = false;
    private string? SuccessMessage = null;
    private string? GeneratedFeatureName = null;

    public CodeCofig? CodeCofig { get; set; }

    public Index()
    {
        CodeCofig = JsonSerializer.Deserialize<CodeCofig>(File.ReadAllText("webportal-profile.json"));
        if (File.Exists("temp.local.json"))
        {
            try
            {
                var jsonText = File.ReadAllText("temp.local.json");
                M = JsonSerializer.Deserialize<FormViewModel>(jsonText) ?? new FormViewModel()
                {
                    PkType = string.Empty,
                    GenerateForm = true,
                    GenerateListing = true,
                    GenerateSelectList = false,
                    SelectListModelType = "SelectOption",
                    SelectListDataType = "string",
                    GenerateControllerAndClientService = true
                };
            }
            catch { }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // Load all previously generated features so the tree is populated immediately.
        var basePath = PathDetectionService.GetFeatureGenerationBasePath();
        _existingPreviews = await FeatureService.GetAllExistingPreviewsAsync(basePath);
        PreviewFiles = _existingPreviews;

        // Check for context parameters from feature creation
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.ContainsKey("createContext") && queryParams["createContext"] == "true")
        {
            await ApplyIntelligentPrePopulation(queryParams);
        }
    }

    private async Task ApplyIntelligentPrePopulation(Dictionary<string, Microsoft.Extensions.Primitives.StringValues> queryParams)
    {
        try
        {
            // Extract context information
            var contextType = queryParams.GetValueOrDefault("contextType").FirstOrDefault() ?? "";
            var contextName = queryParams.GetValueOrDefault("contextName").FirstOrDefault() ?? "";
            var moduleNamespace = queryParams.GetValueOrDefault("moduleNamespace").FirstOrDefault() ?? "";
            var projectNamespace = queryParams.GetValueOrDefault("projectNamespace").FirstOrDefault() ?? "";
            var suggestedPrefix = queryParams.GetValueOrDefault("suggestedPrefix").FirstOrDefault() ?? "";
            var basePath = queryParams.GetValueOrDefault("basePath").FirstOrDefault() ?? "";

            // Apply intelligent defaults based on context
            if (!string.IsNullOrEmpty(moduleNamespace))
            {
                M.NameSpace = moduleNamespace;
            }

            // BasePath is now automatically detected, no need to set it manually

            // Set intelligent defaults based on context type
            switch (contextType)
            {
                case "Module":
                case "Feature":
                case "Project":
                    M.DirectoryName = $"{suggestedPrefix}Feature";
                    break;
            }

            AutoDeriveFromDirectoryName();

            // Automatically show preview for context-aware creation
            await ShowPreview();

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying intelligent pre-population: {ex.Message}");
        }
    }

    public async Task GenerateDynamicCode()
    {
        await File.WriteAllTextAsync("temp.local.json", JsonSerializer.Serialize(M), System.Text.Encoding.UTF8);

        var wooqlawProfile = CodeCofig.Profiles.FirstOrDefault();
        if (wooqlawProfile?.Projects == null) return;

        try
        {
            // Automatically detect the base path
            var detectedBasePath = PathDetectionService.GetFeatureGenerationBasePath();

            // Create feature using the new feature management system
            var feature = await FeatureService.CreateFeatureAsync(
                componentPrefix: M.ComponentPrefix ?? "",
                featurePluralName: M.FeaturePluralName ?? "",
                moduleNamespace: M.NameSpace ?? "",
                projectNamespace: $"{wooqlawProfile.Projects.FirstOrDefault()?.NameSpace}.{M.NameSpace}",
                primaryKeyType: M.PkType ?? "string",
                basePath: detectedBasePath,
                directoryName: M.DirectoryName ?? "",
                hasForm: M.GenerateForm,
                hasListing: M.GenerateListing,
                hasSelectList: M.GenerateSelectList,
                selectListModelType: M.SelectListModelType,
                selectListDataType: M.SelectListDataType,
                projects: wooqlawProfile.Projects.ToList(),
                profileConfiguration: JsonSerializer.Serialize(wooqlawProfile),
                uiFramework: wooqlawProfile.UIFramework ?? "Bootstrap"
            );

            // Show success message (auto-hides after 5 s)
            GeneratedFeatureName = M.ComponentPrefix;
            SuccessMessage = $"Successfully generated {M.ComponentPrefix} slice!";
            ShowSuccessMessage = true;
            ShowFilePreview = false;
            StateHasChanged();

            _ = Task.Delay(5000).ContinueWith(_ => InvokeAsync(() =>
            {
                ShowSuccessMessage = false;
                StateHasChanged();
            }));
        }
        catch (Exception ex)
        {
            // Handle error - could show a toast or error message
            Console.WriteLine($"Error creating feature: {ex.Message}");
        }
    }

    public void StartNewSlice()
    {
        // Reset form to initial state
        M = new FormViewModel()
        {
            PkType = string.Empty,
            GenerateForm = true,
            GenerateListing = true,
            GenerateSelectList = false,
            SelectListModelType = "SelectOption",
            SelectListDataType = "string",
            GenerateControllerAndClientService = true
        };

        // Hide success message
        ShowSuccessMessage = false;
        ShowFilePreview = false;
        SuccessMessage = null;
        GeneratedFeatureName = null;

        StateHasChanged();
    }

    public void HideSuccessMessage()
    {
        ShowSuccessMessage = false;
        StateHasChanged();
    }

    private void AutoDeriveFromDirectoryName()
    {
        if (string.IsNullOrEmpty(M.DirectoryName)) return;
        var segments = M.DirectoryName.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0) return;
        var last = segments[^1];
        M.ComponentPrefix = last;
        M.FeaturePluralName = last + "s";
    }

    public async Task HandlePicker(PickerContext ctx)
    {
        M.DirectoryName = ctx.DirectoryName;
        M.NameSpace = ctx.ModuleNamespace;
        AutoDeriveFromDirectoryName();

        if (!string.IsNullOrEmpty(ctx.ComponentPrefix))
        {
            M.ComponentPrefix = ctx.ComponentPrefix;
            M.FeaturePluralName = ctx.ComponentPrefix + "s";
        }

        StateHasChanged();
        await TriggerDebouncedPreview();
    }

    private async Task GenerateSliceFilesWithTemplateEngine(
        string projectType,
        string sliceType,
        string projectPath,
        Dictionary<string, object> parameters)
    {
        try
        {
            var processedFiles = await TemplateEngine.ProcessTemplatesAsync(projectType, sliceType, parameters);

            foreach (var file in processedFiles)
            {
                var fileName = Path.Combine(projectPath, file.Key);
                var directory = Path.GetDirectoryName(fileName);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Normalize line endings to CRLF for Windows compatibility
                var normalizedContent = file.Value.Replace("\r\n", "\n").Replace("\n", "\r\n");
                await File.WriteAllTextAsync(fileName, normalizedContent, System.Text.Encoding.UTF8);
            }
        }
        catch (DirectoryNotFoundException)
        {
            // Template directory doesn't exist for this project type/slice combination
            // This is expected for some combinations, so we can safely ignore
        }
    }

    private string? GetTemplateDirectoryName(ProjectType projectType)
    {
        return projectType switch
        {
            ProjectType.ServiceContracts => "ServiceContracts",
            ProjectType.ServerSideServices => "ServerSideServices",
            ProjectType.Controllers => "Controllers",
            ProjectType.UILibrary => "RazorComponents",
            ProjectType.ClientShared => "ClientShared",
            _ => null // Unsupported project types
        };
    }

    private async Task ShowPreview()
    {
        if (!IsFormValid()) return;

        IsPreviewLoading = true;
        ShowFilePreview = true;
        ShowPlacementGuidance = true;
        StateHasChanged();

        try
        {
            var wooqlawProfile = CodeCofig.Profiles.FirstOrDefault();
            if (wooqlawProfile?.Projects != null)
            {
                // Automatically detect the base path
                var detectedBasePath = PathDetectionService.GetFeatureGenerationBasePath();

                // Generate file preview for the new slice being configured
                var newPreviews = await FeatureService.PreviewFeatureFilesAsync(
                    componentPrefix: M.ComponentPrefix ?? "",
                    featurePluralName: M.FeaturePluralName ?? "",
                    moduleNamespace: M.NameSpace ?? "",
                    projectNamespace: $"{wooqlawProfile.Projects.FirstOrDefault()?.NameSpace}.{M.NameSpace}",
                    primaryKeyType: M.PkType ?? "string",
                    basePath: detectedBasePath,
                    directoryName: M.DirectoryName ?? "",
                    hasForm: M.GenerateForm,
                    hasListing: M.GenerateListing,
                    hasSelectList: M.GenerateSelectList,
                    selectListModelType: M.SelectListModelType,
                    selectListDataType: M.SelectListDataType,
                    projects: wooqlawProfile.Projects.ToList()
                );

                // Merge: start from all existing, remove any that the new slice overwrites,
                // then prepend the new ones so they appear first within their project group.
                var newFilePaths = newPreviews
                    .Select(f => f.FilePath)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
                var merged = _existingPreviews
                    .Where(f => !newFilePaths.Contains(f.FilePath))
                    .ToList();
                merged.InsertRange(0, newPreviews);
                PreviewFiles = merged;

                // Generate placement guidance
                if (PreviewFiles != null)
                {
                    PlacementGuidance = await PlacementService.AnalyzePlacementAsync(
                        componentPrefix: M.ComponentPrefix ?? "",
                        moduleNamespace: M.NameSpace ?? "",
                        projectNamespace: $"{wooqlawProfile.Projects.FirstOrDefault()?.NameSpace}.{M.NameSpace}",
                        basePath: "",
                        newFiles: PreviewFiles
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating preview: {ex.Message}");
            PreviewFiles = new List<FeatureFilePreview>();
            PlacementGuidance = null;
        }
        finally
        {
            IsPreviewLoading = false;
            StateHasChanged();
        }
    }

    private void HidePreview()
    {
        ShowFilePreview = false;
        ShowPlacementGuidance = false;
        PreviewFiles = _existingPreviews;
        PlacementGuidance = null;
        StateHasChanged();
    }

    public async Task TriggerDebouncedPreview()
    {
        _previewDebounceTokenSource?.Cancel();
        _previewDebounceTokenSource?.Dispose();
        _previewDebounceTokenSource = new CancellationTokenSource();
        var token = _previewDebounceTokenSource.Token;

        try
        {
            await Task.Delay(500, token);
            if (token.IsCancellationRequested) return;

            if (IsFormValid())
                await ShowPreview();
            else
            {
                // Form incomplete — fall back to showing only existing components
                PreviewFiles = _existingPreviews;
                StateHasChanged();
            }
        }
        catch (OperationCanceledException) { }
    }

    private bool IsFormValid()
    {
        return !string.IsNullOrEmpty(M.DirectoryName) &&
               !string.IsNullOrEmpty(M.NameSpace) &&
               !string.IsNullOrEmpty(M.PkType) &&
               (M.GenerateForm || M.GenerateListing || M.GenerateSelectList);
    }

}
public class FormViewModel
{
    // public bool ValidationForCMS { get; set; }
    public FormViewModel()
    {

    }

    [Required]
    public string? DirectoryName { get; set; }

    public string NameSpace { get; set; }

    [Required]
    public string? ComponentPrefix { get; set; }

    [Required]
    public string? FeaturePluralName { get; set; }

    public bool GenerateListing { get; set; }

    public bool GenerateControllerAndClientService { get; set; } = true;

    public bool GenerateForm { get; set; }

    public bool GenerateSelectList { get; set; }

    public string SelectListModelType { get; set; } = "SelectOption"; // "SelectOption" or "Custom"

    public string SelectListDataType { get; set; } = "string"; // Used when SelectListModelType is "SelectOption"

    [Required]
    public string? PkType { get; set; } = "string";
}



public class CodeCofig
{
    public List<CodeProfile> Profiles { get; set; }

    public Dictionary<string, List<CodeFile>> ProjectFiles { get; set; }
}

public class CodeProfile
{
    public string Name { get; set; }
    public string FrameworkNamespaces { get; set; }

    public string DbContextNamespaces { get; set; }

    public string UIFramework { get; set; } = "Bootstrap";

    public List<Project>? Projects { get; set; }

}

public class Project
{

    public string? Path { get; set; }

    public string? NameSpace { get; set; }

    public ProjectType ProjectType { get; set; }
}

public class CodeFile
{
    public string? FileName { get; set; }

    public FileCategory FileCategory { get; set; }

    public List<string> Content { get; set; } = new List<string>();
}

public enum ProjectType
{
    ServiceContracts = 2,
    ServerSideServices = 3,
    ClientSideServices = 4,
    Controllers = 5,
    UILibrary = 6,
    CloudFunctions = 7,
    ClientShared = 8,
    ClientSharedDependency = 9,
    ServerSideSharedDependency = 10

}
public enum FileCategory
{
    Listing = 1,
    Form = 2
}