using {{RootNamespace}}.SliceFactory.Data;
using {{RootNamespace}}.SliceFactory.Models;
using {{RootNamespace}}.SliceFactory.Components.Pages;
using System.Text.Json;

namespace {{RootNamespace}}.SliceFactory.Services;

public class FeatureManagementService
{
    private readonly JsonFeatureStore _store;
    private readonly TemplateEngineService _templateEngine;
    private readonly RegistrationManagementService _registrationService;
    private readonly NavigationManagementService _navigationService;

    public FeatureManagementService(
        JsonFeatureStore store,
        TemplateEngineService templateEngine,
        RegistrationManagementService registrationService,
        NavigationManagementService navigationService)
    {
        _store = store;
        _templateEngine = templateEngine;
        _registrationService = registrationService;
        _navigationService = navigationService;
    }

    /// <summary>
    /// Get all features ordered by module then component prefix
    /// </summary>
    public Task<List<Feature>> GetAllFeaturesAsync()
    {
        var result = _store.Features
            .OrderBy(f => f.ModuleNamespace)
            .ThenBy(f => f.ComponentPrefix)
            .ToList();
        return Task.FromResult(result);
    }

    /// <summary>
    /// Get feature by ID with all related data
    /// </summary>
    public Task<Feature?> GetFeatureByIdAsync(string id)
    {
        var feature = _store.Features.FirstOrDefault(f => f.Id == id);
        return Task.FromResult(feature);
    }

    /// <summary>
    /// Get features by module namespace
    /// </summary>
    public Task<List<Feature>> GetFeaturesByModuleAsync(string moduleNamespace)
    {
        var result = _store.Features
            .Where(f => f.ModuleNamespace == moduleNamespace)
            .OrderBy(f => f.ComponentPrefix)
            .ToList();
        return Task.FromResult(result);
    }

    /// <summary>
    /// Create a new feature and generate its files
    /// </summary>
    public async Task<Feature> CreateFeatureAsync(
        string componentPrefix,
        string featurePluralName,
        string moduleNamespace,
        string projectNamespace,
        string primaryKeyType,
        string basePath,
        string directoryName,
        bool hasForm,
        bool hasListing,
        bool hasSelectList,
        string selectListModelType,
        string selectListDataType,
        List<Project> projects,
        string? profileConfiguration = null,
        string uiFramework = "Bootstrap")
    {
        // Check if feature already exists
        var existingFeature = _store.Features
            .FirstOrDefault(f => f.ModuleNamespace == moduleNamespace && f.ComponentPrefix == componentPrefix);

        if (existingFeature != null)
        {
            throw new InvalidOperationException($"Feature '{componentPrefix}' already exists in module '{moduleNamespace}'");
        }

        var feature = new Feature
        {
            ComponentPrefix = componentPrefix,
            FeaturePluralName = featurePluralName,
            ModuleNamespace = moduleNamespace,
            ProjectNamespace = projectNamespace,
            PrimaryKeyType = primaryKeyType,
            BasePath = basePath,
            DirectoryName = directoryName,
            HasForm = hasForm,
            HasListing = hasListing,
            HasSelectList = hasSelectList,
            SelectListModelType = selectListModelType,
            SelectListDataType = selectListDataType,
            UIFramework = uiFramework,
            ProfileConfiguration = profileConfiguration,
            CreatedAt = DateTime.UtcNow
        };

        _store.Features.Add(feature);

        // Generate files for each project
        await GenerateFeatureFilesAsync(feature, projects);

        // Save persists the feature plus all generated file/project records
        await _store.SaveAsync();

        return feature;
    }

    /// <summary>
    /// Update an existing feature and regenerate files if needed
    /// </summary>
    public async Task<Feature> UpdateFeatureAsync(
        string featureId,
        string componentPrefix,
        string featurePluralName,
        string moduleNamespace,
        string projectNamespace,
        string primaryKeyType,
        string basePath,
        string directoryName,
        bool hasForm,
        bool hasListing,
        List<Project> projects,
        bool regenerateFiles = false)
    {
        var feature = await GetFeatureByIdAsync(featureId);
        if (feature == null)
        {
            throw new ArgumentException($"Feature with ID {featureId} not found");
        }

        // Update feature properties
        feature.ComponentPrefix = componentPrefix;
        feature.FeaturePluralName = featurePluralName;
        feature.ModuleNamespace = moduleNamespace;
        feature.ProjectNamespace = projectNamespace;
        feature.PrimaryKeyType = primaryKeyType;
        feature.BasePath = basePath;
        feature.DirectoryName = directoryName;
        feature.HasForm = hasForm;
        feature.HasListing = hasListing;
        feature.UpdatedAt = DateTime.UtcNow;

        if (regenerateFiles)
        {
            feature.Files.Clear();
            feature.Projects.Clear();
            await GenerateFeatureFilesAsync(feature, projects);
        }

        await _store.SaveAsync();
        return feature;
    }

    /// <summary>
    /// Delete a feature and optionally remove generated files
    /// </summary>
    public async Task DeleteFeatureAsync(string featureId, bool deleteFiles = false)
    {
        var feature = await GetFeatureByIdAsync(featureId);
        if (feature == null)
        {
            throw new ArgumentException($"Feature with ID {featureId} not found");
        }

        if (deleteFiles)
        {
            foreach (var file in feature.Files.Where(f => f.Exists))
            {
                try
                {
                    if (File.Exists(file.FilePath))
                        File.Delete(file.FilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {file.FilePath}: {ex.Message}");
                }
            }
        }

        _store.Features.Remove(feature);
        await _store.SaveAsync();
    }

    /// <summary>
    /// Preview files that would be generated for a feature
    /// </summary>
    public async Task<List<FeatureFilePreview>> PreviewFeatureFilesAsync(
        string componentPrefix,
        string featurePluralName,
        string moduleNamespace,
        string projectNamespace,
        string primaryKeyType,
        string basePath,
        string directoryName,
        bool hasForm,
        bool hasListing,
        bool hasSelectList,
        string selectListModelType,
        string selectListDataType,
        List<Project> projects)
    {
        var previews = new List<FeatureFilePreview>();
        var parameters = _templateEngine.CreateParameterDictionary(componentPrefix, moduleNamespace, projectNamespace, primaryKeyType, null, selectListModelType, selectListDataType);

        foreach (var project in projects)
        {
            var templateDirectoryName = GetTemplateDirectoryName(project.ProjectType);
            if (string.IsNullOrEmpty(templateDirectoryName))
                continue;

            var projectRootPath = Path.Combine(basePath, project.Path ?? "");
            var projectNs = project.NameSpace ?? "";
            var baseFeaturePath = Path.Combine(projectRootPath, directoryName);

            if (hasListing)
            {
                var listingPath = Path.Combine(baseFeaturePath, $"{featurePluralName}Listing");
                var listingFiles = await GetTemplateFilesPreviewAsync(templateDirectoryName, "Listing", listingPath, parameters);
                foreach (var f in listingFiles)
                {
                    var fp = Path.Combine(listingPath, f.Key);
                    previews.Add(new FeatureFilePreview
                    {
                        ProjectType = templateDirectoryName,
                        SliceType = "Listing",
                        FileName = f.Key,
                        FilePath = fp,
                        DirectoryPath = Path.GetDirectoryName(fp) ?? "",
                        ProjectRootPath = projectRootPath,
                        ProjectNamespace = projectNs,
                        ModuleNamespace = moduleNamespace,
                        ComponentPrefix = componentPrefix,
                        IsNew = true,
                        Content = f.Value
                    });
                }
            }

            if (hasForm)
            {
                var formPath = Path.Combine(baseFeaturePath, $"{componentPrefix}Form");
                var formFiles = await GetTemplateFilesPreviewAsync(templateDirectoryName, "Form", formPath, parameters);
                foreach (var f in formFiles)
                {
                    var fp = Path.Combine(formPath, f.Key);
                    previews.Add(new FeatureFilePreview
                    {
                        ProjectType = templateDirectoryName,
                        SliceType = "Form",
                        FileName = f.Key,
                        FilePath = fp,
                        DirectoryPath = Path.GetDirectoryName(fp) ?? "",
                        ProjectRootPath = projectRootPath,
                        ProjectNamespace = projectNs,
                        ModuleNamespace = moduleNamespace,
                        ComponentPrefix = componentPrefix,
                        IsNew = true,
                        Content = f.Value
                    });
                }
            }

            if (hasSelectList)
            {
                var selectListPath = Path.Combine(baseFeaturePath, $"{featurePluralName}SelectList");
                var selectListFiles = await GetTemplateFilesPreviewAsync(templateDirectoryName, "SelectList", selectListPath, parameters);
                foreach (var f in selectListFiles)
                {
                    var fp = Path.Combine(selectListPath, f.Key);
                    previews.Add(new FeatureFilePreview
                    {
                        ProjectType = templateDirectoryName,
                        SliceType = "SelectList",
                        FileName = f.Key,
                        FilePath = fp,
                        DirectoryPath = Path.GetDirectoryName(fp) ?? "",
                        ProjectRootPath = projectRootPath,
                        ProjectNamespace = projectNs,
                        ModuleNamespace = moduleNamespace,
                        ComponentPrefix = componentPrefix,
                        IsNew = true,
                        Content = f.Value
                    });
                }
            }
        }

        return previews;
    }

    /// <summary>
    /// Build hierarchical tree structure for features
    /// </summary>
    public async Task<List<FeatureTreeNode>> GetFeatureTreeAsync()
    {
        var features = await GetAllFeaturesAsync();
        var tree = new List<FeatureTreeNode>();

        foreach (var moduleGroup in features.GroupBy(f => f.ModuleNamespace))
        {
            var moduleNode = new FeatureTreeNode
            {
                Id = $"module_{moduleGroup.Key}",
                Name = moduleGroup.Key,
                Type = "Module",
                ModuleNamespace = moduleGroup.Key,
                IsExpanded = false
            };

            foreach (var feature in moduleGroup)
            {
                var featureNode = new FeatureTreeNode
                {
                    Id = $"feature_{feature.Id}",
                    Name = feature.ComponentPrefix,
                    Type = "Feature",
                    Feature = feature,
                    IsExpanded = false
                };

                foreach (var projectGroup in feature.Files.GroupBy(f => f.ProjectType))
                {
                    var projectNode = new FeatureTreeNode
                    {
                        Id = $"project_{feature.Id}_{projectGroup.Key}",
                        Name = projectGroup.Key,
                        Type = "ProjectType",
                        Feature = feature,
                        IsExpanded = false
                    };

                    foreach (var file in projectGroup)
                    {
                        projectNode.Children.Add(new FeatureTreeNode
                        {
                            Id = $"file_{file.Id}",
                            Name = $"{file.FileName} ({file.SliceType})",
                            Type = "File",
                            File = file,
                            Feature = feature
                        });
                    }

                    featureNode.Children.Add(projectNode);
                }

                moduleNode.Children.Add(featureNode);
            }

            tree.Add(moduleNode);
        }

        return tree;
    }

    private async Task GenerateFeatureFilesAsync(Feature feature, List<Project> projects)
    {
        var parameters = _templateEngine.CreateParameterDictionary(
            feature.ComponentPrefix,
            feature.ModuleNamespace,
            feature.ProjectNamespace,
            feature.PrimaryKeyType,
            feature.UIFramework,
            feature.SelectListModelType,
            feature.SelectListDataType);

        foreach (var project in projects)
        {
            var templateDirectoryName = GetTemplateDirectoryName(project.ProjectType);
            if (string.IsNullOrEmpty(templateDirectoryName))
                continue;

            var baseFeaturePath = Path.Combine(feature.BasePath, project.Path, feature.DirectoryName);

            feature.Projects.Add(new FeatureProject
            {
                FeatureId = feature.Id,
                ProjectType = project.ProjectType,
                // Store relative to solution root so paths are portable across machines
                ProjectPath = Path.GetRelativePath(feature.BasePath, baseFeaturePath),
                ProjectNamespace = project.NameSpace ?? "",
                CreatedAt = DateTime.UtcNow
            });

            if (feature.HasListing)
            {
                var listingPath = Path.Combine(baseFeaturePath, $"{feature.FeaturePluralName}Listing");
                await GenerateSliceFilesAsync(feature, templateDirectoryName, "Listing", listingPath, parameters);
            }

            if (feature.HasForm)
            {
                var formPath = Path.Combine(baseFeaturePath, $"{feature.ComponentPrefix}Form");
                await GenerateSliceFilesAsync(feature, templateDirectoryName, "Form", formPath, parameters);
            }

            if (feature.HasSelectList)
            {
                var selectListPath = Path.Combine(baseFeaturePath, $"{feature.FeaturePluralName}SelectList");
                await GenerateSliceFilesAsync(feature, templateDirectoryName, "SelectList", selectListPath, parameters);
            }
        }

        // Update service registrations and navigation menus after generating files
        await _registrationService.UpdateRegistrationsForFeatureAsync(feature, projects);
        await _navigationService.UpdateNavigationForFeatureAsync(feature, projects);
    }

    private async Task GenerateSliceFilesAsync(
        Feature feature,
        string projectType,
        string sliceType,
        string projectPath,
        Dictionary<string, object> parameters)
    {
        try
        {
            var processedFiles = await _templateEngine.ProcessTemplatesAsync(projectType, sliceType, parameters);

            foreach (var file in processedFiles)
            {
                var fullPath = Path.Combine(projectPath, file.Key);
                var directory = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                // Normalize line endings to CRLF for Windows compatibility
                var normalizedContent = file.Value.Replace("\r\n", "\n").Replace("\n", "\r\n");
                await File.WriteAllTextAsync(fullPath, normalizedContent, System.Text.Encoding.UTF8);

                feature.Files.Add(new FeatureFile
                {
                    FeatureId = feature.Id,
                    // Store relative to solution root so paths are portable across machines
                    FilePath = Path.GetRelativePath(feature.BasePath, fullPath),
                    FileName = file.Key,
                    ProjectType = projectType,
                    SliceType = sliceType,
                    FileSize = System.Text.Encoding.UTF8.GetByteCount(file.Value),
                    CreatedAt = DateTime.UtcNow,
                    Exists = true
                });
            }
        }
        catch (DirectoryNotFoundException)
        {
            // Template directory doesn't exist for this project type/slice combination — expected for some combinations
        }
    }

    private async Task<Dictionary<string, string>> GetTemplateFilesPreviewAsync(
        string projectType,
        string sliceType,
        string projectPath,
        Dictionary<string, object> parameters)
    {
        try
        {
            return await _templateEngine.ProcessTemplatesAsync(projectType, sliceType, parameters);
        }
        catch (DirectoryNotFoundException)
        {
            return new Dictionary<string, string>();
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
            _ => null
        };
    }

    /// <summary>
    /// Returns previews for ALL previously-generated features so the tree is populated immediately.
    /// Relative file paths stored in JSON are reconstructed to absolute using <paramref name="basePath"/>.
    /// </summary>
    public Task<List<FeatureFilePreview>> GetAllExistingPreviewsAsync(string basePath)
    {
        var result = new List<FeatureFilePreview>();

        foreach (var feature in _store.Features)
        {
            foreach (var file in feature.Files)
            {
                var featureProject = feature.Projects.FirstOrDefault(p =>
                    GetTemplateDirectoryName(p.ProjectType) == file.ProjectType);

                var projectRootPath = string.Empty;
                var projectNamespace = string.Empty;

                if (featureProject != null)
                {
                    // ProjectPath is stored relative to basePath — reconstruct absolute
                    var absoluteProjectPath = Path.Combine(basePath, featureProject.ProjectPath);
                    projectRootPath = StripDirectoryNameFromPath(absoluteProjectPath, feature.DirectoryName);
                    projectNamespace = featureProject.ProjectNamespace;
                }

                // FilePath stored relative to basePath — reconstruct absolute
                var absoluteFilePath = Path.Combine(basePath, file.FilePath);
                var absoluteDir = Path.GetDirectoryName(absoluteFilePath) ?? "";

                result.Add(new FeatureFilePreview
                {
                    ProjectType = file.ProjectType,
                    SliceType = file.SliceType,
                    FileName = file.FileName,
                    FilePath = absoluteFilePath,
                    DirectoryPath = absoluteDir,
                    Content = string.Empty,
                    ProjectRootPath = projectRootPath,
                    IsNew = false,
                    ProjectNamespace = projectNamespace,
                    ModuleNamespace = feature.ModuleNamespace,
                    ComponentPrefix = feature.ComponentPrefix
                });
            }
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Strips the feature's directoryName path segments from the end of featurePath
    /// to recover the project root (basePath + projectRelPath).
    /// </summary>
    private static string StripDirectoryNameFromPath(string featurePath, string directoryName)
    {
        var normalized = featurePath.TrimEnd('\\', '/');
        var dirSuffix = directoryName.TrimEnd('\\', '/');
        var normPath = normalized.Replace('/', '\\');
        var normDir = dirSuffix.Replace('/', '\\');
        if (normPath.EndsWith(normDir, StringComparison.OrdinalIgnoreCase))
            return normalized[..^dirSuffix.Length].TrimEnd('\\', '/');
        return normalized;
    }
}

/// <summary>
/// Preview of a file that would be generated
/// </summary>
public class FeatureFilePreview
{
    public string ProjectType { get; set; } = string.Empty;
    public string SliceType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string DirectoryPath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The project root path (basePath + project.Path). The tree strips this prefix
    /// so that directoryName segments are always visible as tree nodes.
    /// </summary>
    public string ProjectRootPath { get; set; } = string.Empty;

    /// <summary>
    /// True for files about to be generated; false for files already on disk.
    /// </summary>
    public bool IsNew { get; set; } = true;

    /// <summary>
    /// Base namespace of the owning project (e.g. "MyApp.ServiceContracts").
    /// </summary>
    public string ProjectNamespace { get; set; } = string.Empty;

    /// <summary>
    /// The feature's module namespace (e.g. "Profile.Article").
    /// </summary>
    public string ModuleNamespace { get; set; } = string.Empty;

    /// <summary>
    /// The feature's component prefix (e.g. "Article").
    /// Used by the tree picker to pre-populate the Component Prefix input.
    /// </summary>
    public string ComponentPrefix { get; set; } = string.Empty;
}

/// <summary>
/// Context passed to the form when the user picks an existing tree node as a sibling template.
/// </summary>
/// <param name="DirectoryName">Feature directory path relative to project root (e.g. "Profile\Article").</param>
/// <param name="ModuleNamespace">Module namespace (e.g. "Profile.Article").</param>
/// <param name="ComponentPrefix">Set when a file node is clicked; null for directory picks.</param>
public record PickerContext(string DirectoryName, string ModuleNamespace, string? ComponentPrefix);
