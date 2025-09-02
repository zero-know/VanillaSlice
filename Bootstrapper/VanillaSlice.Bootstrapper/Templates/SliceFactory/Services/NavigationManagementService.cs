using {{RootNamespace}}.SliceFactory.Models;
using {{RootNamespace}}.SliceFactory.Components.Pages;

namespace {{RootNamespace}}.SliceFactory.Services;

/// <summary>
/// Service responsible for automatically managing navigation menu items using placeholder replacement
/// </summary>
public class NavigationManagementService
{
    private readonly ILogger<NavigationManagementService> _logger;
    private readonly PluralizationService _pluralizationService;
    private const string NAVIGATION_PLACEHOLDER = "@* ##MenuItem## *@";

    public NavigationManagementService(
        ILogger<NavigationManagementService> logger,
        PluralizationService pluralizationService)
    {
        _logger = logger;
        _pluralizationService = pluralizationService;
    }

    /// <summary>
    /// Updates navigation menu files for a newly created feature using placeholder replacement
    /// </summary>
    public async Task UpdateNavigationForFeatureAsync(Feature feature, List<Project> projects)
    {
        try
        {
            _logger.LogInformation("Updating navigation menus for feature: {ComponentPrefix}", feature.ComponentPrefix);

            // Only add navigation items for features that have listings
            if (!feature.HasListing)
            {
                _logger.LogInformation("Feature {ComponentPrefix} has no listing, skipping navigation update", feature.ComponentPrefix);
                return;
            }

            foreach (var project in projects)
            {
                if (project.ProjectType == ProjectType.UILibrary)
                {
                    await UpdateNavigationMenuAsync(feature, project);
                }
            }

            // Also update navigation in WebPortal and HybridApp projects directly
            await UpdateWebPortalNavigationAsync(feature);
            await UpdateHybridAppNavigationAsync(feature);

            _logger.LogInformation("Successfully updated navigation menus for feature: {ComponentPrefix}", feature.ComponentPrefix);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update navigation menus for feature: {ComponentPrefix}", feature.ComponentPrefix);
            throw;
        }
    }

    /// <summary>
    /// Removes navigation entries for a deleted feature (placeholder-based approach doesn't support removal)
    /// </summary>
    public async Task RemoveNavigationForFeatureAsync(Feature feature, List<Project> projects)
    {
        _logger.LogWarning("Navigation removal not supported with placeholder-based approach for feature: {ComponentPrefix}", feature.ComponentPrefix);
        // Note: With placeholder approach, we don't remove navigation items automatically
        // This would require manual cleanup or a more sophisticated approach
        await Task.CompletedTask;
    }

    private async Task UpdateNavigationMenuAsync(Feature feature, Project project)
    {
        var navMenuPath = GetNavMenuFilePath(project, feature.BasePath);
        if (!File.Exists(navMenuPath))
        {
            _logger.LogWarning("Navigation menu file not found: {FilePath}", navMenuPath);
            return;
        }

        var navigationItem = GenerateNavigationItem(feature);
        await UpdateNavigationFileWithPlaceholderAsync(navMenuPath, navigationItem, feature.ComponentPrefix);
    }

    private string GetNavMenuFilePath(Project project, string basePath)
    {
        var projectPath = project.Path;

        if (project.ProjectType == ProjectType.UILibrary)
        {
            // For UI Library projects, we need to determine the correct path
            // This might need to be adjusted based on the actual project structure
            return Path.Combine(basePath, projectPath ?? "", "Components", "Layout", "NavMenu.razor");
        }

        return string.Empty;
    }

    private string GenerateNavigationItem(Feature feature)
    {
        var pluralizedRoute = _pluralizationService.Pluralize(feature.ComponentPrefix).ToLowerInvariant();
        var displayName = feature.ComponentPrefix; // Keep singular for display

        return $@"        <div class=""nav-item px-3"">
            <NavLink class=""nav-link"" href=""{pluralizedRoute}"">
                <span class=""bi bi-list-nested-nav-menu"" aria-hidden=""true""></span> {displayName}
            </NavLink>
        </div>";
    }

    private async Task UpdateWebPortalNavigationAsync(Feature feature)
    {
        var webPortalNavMenuPath = Path.Combine(feature.BasePath,
            "{{ProjectName}}.WebPortal",
            "{{ProjectName}}.WebPortal.Client",
            "Layout",
            "NavMenu.razor");

        if (File.Exists(webPortalNavMenuPath))
        {
            var navigationItem = GenerateNavigationItem(feature);
            await UpdateNavigationFileWithPlaceholderAsync(webPortalNavMenuPath, navigationItem, feature.ComponentPrefix);
        }
        else
        {
            _logger.LogWarning("WebPortal NavMenu file not found: {FilePath}", webPortalNavMenuPath);
        }
    }

    private async Task UpdateHybridAppNavigationAsync(Feature feature)
    {
        var hybridAppNavMenuPath = Path.Combine(feature.BasePath,
            "{{ProjectName}}.HybridApp",
            "Components",
            "Layout",
            "NavMenu.razor");

        if (File.Exists(hybridAppNavMenuPath))
        {
            var navigationItem = GenerateNavigationItem(feature);
            await UpdateNavigationFileWithPlaceholderAsync(hybridAppNavMenuPath, navigationItem, feature.ComponentPrefix);
        }
        else
        {
            _logger.LogWarning("HybridApp NavMenu file not found: {FilePath}", hybridAppNavMenuPath);
        }
    }

    private async Task UpdateNavigationFileWithPlaceholderAsync(string filePath, string newNavigationItem, string componentPrefix)
    {
        try
        {
            var content = await File.ReadAllTextAsync(filePath);

            if (!content.Contains(NAVIGATION_PLACEHOLDER))
            {
                _logger.LogWarning("Navigation placeholder {Placeholder} not found in file: {FilePath}", NAVIGATION_PLACEHOLDER, filePath);
                return;
            }

            // Find the placeholder with its current indentation and replace it
            var lines = content.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == NAVIGATION_PLACEHOLDER)
                {
                    // Get the current indentation
                    var indentation = lines[i].Substring(0, lines[i].IndexOf(NAVIGATION_PLACEHOLDER));

                    // Build the replacement with proper indentation
                    var indentedNavigationItem = newNavigationItem.Replace("        ", indentation + "        ");
                    var finalReplacement = indentedNavigationItem + Environment.NewLine + 
                                         Environment.NewLine + indentation + NAVIGATION_PLACEHOLDER;

                    lines[i] = finalReplacement;
                    break;
                }
            }

            var updatedContent = string.Join(Environment.NewLine, lines);
            await File.WriteAllTextAsync(filePath, updatedContent);

            _logger.LogInformation("Updated navigation file: {FilePath} with navigation item for {ComponentPrefix}",
                filePath, componentPrefix);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update navigation file: {FilePath}", filePath);
            throw;
        }
    }
}
