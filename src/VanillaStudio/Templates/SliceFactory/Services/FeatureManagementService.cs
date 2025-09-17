using Microsoft.EntityFrameworkCore;
using {{RootNamespace}}.SliceFactory.Data;
using {{RootNamespace}}.SliceFactory.Models;
using {{RootNamespace}}.SliceFactory.Components.Pages;
using System.Text.Json;

namespace {{RootNamespace}}.SliceFactory.Services;

public class FeatureManagementService
{
    private readonly SliceFactoryDbContext _context;
    private readonly TemplateEngineService _templateEngine;
    private readonly RegistrationManagementService _registrationService;
    private readonly NavigationManagementService _navigationService;

    public FeatureManagementService(
        SliceFactoryDbContext context,
        TemplateEngineService templateEngine,
        RegistrationManagementService registrationService,
        NavigationManagementService navigationService)
    {
        _context = context;
        _templateEngine = templateEngine;
        _registrationService = registrationService;
        _navigationService = navigationService;
    }

    /// <summary>
    /// Get all features with their files and projects
    /// </summary>
    public async Task<List<Feature>> GetAllFeaturesAsync()
    {
        return await _context.Features
            .Include(f => f.Files)
            .Include(f => f.Projects)
            .OrderBy(f => f.ModuleNamespace)
            .ThenBy(f => f.ComponentPrefix)
            .ToListAsync();
    }

    /// <summary>
    /// Get feature by ID with all related data
    /// </summary>
    public async Task<Feature?> GetFeatureByIdAsync(int id)
    {
        return await _context.Features
            .Include(f => f.Files)
            .Include(f => f.Projects)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    /// <summary>
    /// Get features by module namespace
    /// </summary>
    public async Task<List<Feature>> GetFeaturesByModuleAsync(string moduleNamespace)
    {
        return await _context.Features
            .Include(f => f.Files)
            .Include(f => f.Projects)
            .Where(f => f.ModuleNamespace == moduleNamespace)
            .OrderBy(f => f.ComponentPrefix)
            .ToListAsync();
    }

    /// <summary>
    /// Create a new feature and generate its files
    /// </summary>
    public async Task<Feature> CreateFeatureAsync(
        string name,
        string componentPrefix,
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
        var existingFeature = await _context.Features
            .FirstOrDefaultAsync(f => f.ModuleNamespace == moduleNamespace && f.ComponentPrefix == componentPrefix);

        if (existingFeature != null)
        {
            throw new InvalidOperationException($"Feature '{componentPrefix}' already exists in module '{moduleNamespace}'");
        }

        var feature = new Feature
        {
            Name = name,
            ComponentPrefix = componentPrefix,
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

        _context.Features.Add(feature);
        await _context.SaveChangesAsync();

        // Generate files for each project
        await GenerateFeatureFilesAsync(feature, projects);

        return feature;
    }

    /// <summary>
    /// Update an existing feature and regenerate files if needed
    /// </summary>
    public async Task<Feature> UpdateFeatureAsync(
        int featureId,
        string name,
        string componentPrefix,
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
        feature.Name = name;
        feature.ComponentPrefix = componentPrefix;
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
            // Remove old files from database (but not from disk)
            _context.FeatureFiles.RemoveRange(feature.Files);
            _context.FeatureProjects.RemoveRange(feature.Projects);

            await _context.SaveChangesAsync();

            // Generate new files
            await GenerateFeatureFilesAsync(feature, projects);
        }
        else
        {
            await _context.SaveChangesAsync();
        }

        return feature;
    }

    /// <summary>
    /// Delete a feature and optionally remove generated files
    /// </summary>
    public async Task DeleteFeatureAsync(int featureId, bool deleteFiles = false)
    {
        var feature = await GetFeatureByIdAsync(featureId);
        if (feature == null)
        {
            throw new ArgumentException($"Feature with ID {featureId} not found");
        }

        if (deleteFiles)
        {
            // Delete physical files
            foreach (var file in feature.Files.Where(f => f.Exists))
            {
                try
                {
                    if (File.Exists(file.FilePath))
                    {
                        File.Delete(file.FilePath);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue with deletion
                    Console.WriteLine($"Error deleting file {file.FilePath}: {ex.Message}");
                }
            }
        }

        _context.Features.Remove(feature);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Preview files that would be generated for a feature
    /// </summary>
    public async Task<List<FeatureFilePreview>> PreviewFeatureFilesAsync(
        string componentPrefix,
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

            var projectPath = Path.Combine(basePath, project.Path, directoryName);

            if (hasListing)
            {
                var listingFiles = await GetTemplateFilesPreviewAsync(templateDirectoryName, "Listing", projectPath, parameters);
                previews.AddRange(listingFiles.Select(f => new FeatureFilePreview
                {
                    ProjectType = templateDirectoryName,
                    SliceType = "Listing",
                    FileName = f.Key,
                    FilePath = Path.Combine(projectPath, f.Key),
                    DirectoryPath = Path.GetDirectoryName(Path.Combine(projectPath, f.Key)) ?? "",
                    Content = f.Value
                }));
            }

            if (hasForm)
            {
                var formFiles = await GetTemplateFilesPreviewAsync(templateDirectoryName, "Form", projectPath, parameters);
                previews.AddRange(formFiles.Select(f => new FeatureFilePreview
                {
                    ProjectType = templateDirectoryName,
                    SliceType = "Form",
                    FileName = f.Key,
                    FilePath = Path.Combine(projectPath, f.Key),
                    DirectoryPath = Path.GetDirectoryName(Path.Combine(projectPath, f.Key)) ?? "",
                    Content = f.Value
                }));
            }

            if (hasSelectList)
            {
                var selectListFiles = await GetTemplateFilesPreviewAsync(templateDirectoryName, "SelectList", projectPath, parameters);
                previews.AddRange(selectListFiles.Select(f => new FeatureFilePreview
                {
                    ProjectType = templateDirectoryName,
                    SliceType = "SelectList",
                    FileName = f.Key,
                    FilePath = Path.Combine(projectPath, f.Key),
                    DirectoryPath = Path.GetDirectoryName(Path.Combine(projectPath, f.Key)) ?? "",
                    Content = f.Value
                }));
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

        // Group by module namespace
        var moduleGroups = features.GroupBy(f => f.ModuleNamespace);

        foreach (var moduleGroup in moduleGroups)
        {
            var moduleNode = new FeatureTreeNode
            {
                Id = $"module_{moduleGroup.Key}",
                Name = moduleGroup.Key,
                Type = "Module",
                ModuleNamespace = moduleGroup.Key,
                IsExpanded = false
            };

            // Add features under each module
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

                // Add project types under each feature
                var projectGroups = feature.Files.GroupBy(f => f.ProjectType);
                foreach (var projectGroup in projectGroups)
                {
                    var projectNode = new FeatureTreeNode
                    {
                        Id = $"project_{feature.Id}_{projectGroup.Key}",
                        Name = projectGroup.Key,
                        Type = "ProjectType",
                        Feature = feature,
                        IsExpanded = false
                    };

                    // Add files under each project type
                    foreach (var file in projectGroup)
                    {
                        var fileNode = new FeatureTreeNode
                        {
                            Id = $"file_{file.Id}",
                            Name = $"{file.FileName} ({file.SliceType})",
                            Type = "File",
                            File = file,
                            Feature = feature
                        };
                        projectNode.Children.Add(fileNode);
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

            var projectPath = Path.Combine(feature.BasePath, project.Path, feature.DirectoryName);

            // Create FeatureProject record
            var featureProject = new FeatureProject
            {
                FeatureId = feature.Id,
                ProjectType = project.ProjectType,
                ProjectPath = projectPath,
                ProjectNamespace = project.NameSpace ?? "",
                CreatedAt = DateTime.UtcNow
            };
            _context.FeatureProjects.Add(featureProject);

            if (feature.HasListing)
            {
                await GenerateSliceFilesAsync(feature, templateDirectoryName, "Listing", projectPath, parameters);
            }

            if (feature.HasForm)
            {
                await GenerateSliceFilesAsync(feature, templateDirectoryName, "Form", projectPath, parameters);
            }

            if (feature.HasSelectList)
            {
                await GenerateSliceFilesAsync(feature, templateDirectoryName, "SelectList", projectPath, parameters);
            }
        }

        await _context.SaveChangesAsync();

        // Update service registrations after generating files
        await _registrationService.UpdateRegistrationsForFeatureAsync(feature, projects);

        // Update navigation menus after generating files
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
                {
                    Directory.CreateDirectory(directory!);
                }

                // Normalize line endings to CRLF for Windows compatibility
                var normalizedContent = file.Value.Replace("\r\n", "\n").Replace("\n", "\r\n");
                await File.WriteAllTextAsync(fullPath, normalizedContent, System.Text.Encoding.UTF8);

                // Create FeatureFile record
                var featureFile = new FeatureFile
                {
                    FeatureId = feature.Id,
                    FilePath = fullPath,
                    FileName = file.Key,
                    ProjectType = projectType,
                    SliceType = sliceType,
                    FileSize = System.Text.Encoding.UTF8.GetByteCount(file.Value),
                    CreatedAt = DateTime.UtcNow,
                    Exists = true
                };
                _context.FeatureFiles.Add(featureFile);
            }
        }
        catch (DirectoryNotFoundException)
        {
            // Template directory doesn't exist for this project type/slice combination
            // This is expected for some combinations, so we can safely ignore
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
}
