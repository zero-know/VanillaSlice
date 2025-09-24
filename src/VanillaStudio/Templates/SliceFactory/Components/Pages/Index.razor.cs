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
                                       !string.IsNullOrEmpty(M.ComponentPrefix) &&
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
    private bool ShowFilePreview = false;
    private bool IsPreviewLoading = false;
    private List<FeatureFilePreview>? PreviewFiles;

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

            if (!string.IsNullOrEmpty(suggestedPrefix))
            {
                M.ComponentPrefix = suggestedPrefix;
            }

            // BasePath is now automatically detected, no need to set it manually

            // Set intelligent defaults based on context type
            switch (contextType)
            {
                case "Module":
                    // Creating at module level - suggest new feature
                    M.DirectoryName = $"{suggestedPrefix}Feature";
                    break;

                case "Feature":
                    // Creating sibling to existing feature
                    M.DirectoryName = $"{suggestedPrefix}Feature";
                    break;

                case "Project":
                    // Creating at project level - inherit parent settings
                    M.DirectoryName = $"{suggestedPrefix}Feature";
                    break;
            }

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
                name: M.ComponentPrefix ?? "Unknown",
                componentPrefix: M.ComponentPrefix ?? "",
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

            // Show success message
            GeneratedFeatureName = M.ComponentPrefix;
            SuccessMessage = $"✅ Successfully generated {M.ComponentPrefix} slice!";
            ShowSuccessMessage = true;
            ShowFilePreview = false;
            StateHasChanged();
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

                // Generate file preview
                PreviewFiles = await FeatureService.PreviewFeatureFilesAsync(
                    componentPrefix: M.ComponentPrefix ?? "",
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
        PreviewFiles = null;
        PlacementGuidance = null;
        StateHasChanged();
    }

    private bool IsFormValid()
    {
        return !string.IsNullOrEmpty(M.DirectoryName) &&
               !string.IsNullOrEmpty(M.NameSpace) &&
               !string.IsNullOrEmpty(M.ComponentPrefix) &&
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