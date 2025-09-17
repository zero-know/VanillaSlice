using {{RootNamespace}}.SliceFactory.Components.Pages;
using {{RootNamespace}}.SliceFactory.Models;

namespace {{RootNamespace}}.SliceFactory.Services;

/// <summary>
/// Service responsible for automatically managing service registrations using placeholder replacement
/// </summary>
public class RegistrationManagementService
{
    private readonly ILogger<RegistrationManagementService> _logger;
    private const string SERVER_PLACEHOLDER = "//##ServerDataService##";
    private const string CLIENT_PLACEHOLDER = "//##ClientDataService##";

    public RegistrationManagementService(ILogger<RegistrationManagementService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Updates registration files for a newly created feature using placeholder replacement
    /// </summary>
    public async Task UpdateRegistrationsForFeatureAsync(Feature feature, List<Project> projects)
    {
        try
        {
            _logger.LogInformation("Updating service registrations for feature: {ComponentPrefix}", feature.ComponentPrefix);

            foreach (var project in projects)
            {
                if (project.ProjectType == ProjectType.ServerSideServices)
                {
                    await UpdateServerSideRegistrationsAsync(feature, project);
                }
                else if (project.ProjectType == ProjectType.ClientShared)
                {
                    await UpdateClientSideRegistrationsAsync(feature, project);
                }
            }

            _logger.LogInformation("Successfully updated service registrations for feature: {ComponentPrefix}", feature.ComponentPrefix);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update service registrations for feature: {ComponentPrefix}", feature.ComponentPrefix);
            throw;
        }
    }

    /// <summary>
    /// Removes registration entries for a deleted feature (placeholder-based approach doesn't support removal)
    /// </summary>
    public async Task RemoveRegistrationsForFeatureAsync(Feature feature, List<Project> projects)
    {
        _logger.LogWarning("Registration removal not supported with placeholder-based approach for feature: {ComponentPrefix}", feature.ComponentPrefix);
        // Note: With placeholder approach, we don't remove registrations automatically
        // This would require manual cleanup or a more sophisticated approach
        await Task.CompletedTask;
    }

    private async Task UpdateServerSideRegistrationsAsync(Feature feature, Project project)
    {
        var registrationFilePath = GetRegistrationFilePath(project, feature.BasePath);
        if (!File.Exists(registrationFilePath))
        {
            _logger.LogWarning("Server-side registration file not found: {FilePath}", registrationFilePath);
            return;
        }

        var registrations = GenerateServerSideRegistrations(feature);
        await UpdateRegistrationFileWithPlaceholderAsync(registrationFilePath, registrations, SERVER_PLACEHOLDER);
    }

    private async Task UpdateClientSideRegistrationsAsync(Feature feature, Project project)
    {
        var registrationFilePath = GetRegistrationFilePath(project, feature.BasePath);
        if (!File.Exists(registrationFilePath))
        {
            _logger.LogWarning("Client-side registration file not found: {FilePath}", registrationFilePath);
            return;
        }

        var registrations = GenerateClientSideRegistrations(feature);
        await UpdateRegistrationFileWithPlaceholderAsync(registrationFilePath, registrations, CLIENT_PLACEHOLDER);
    }

    private string GetRegistrationFilePath(Project project, string basePath)
    {
        // Navigate up from the Features directory to find the Extensions directory
        var projectPath = project.Path?.Replace("\\Features", "") ?? "";
        return Path.Combine(basePath, projectPath, "Extensions", "FeaturesRegistrationExt.cs");
    }

    private List<string> GenerateServerSideRegistrations(Feature feature)
    {
        var registrations = new List<string>();
        var moduleNamespace = feature.ModuleNamespace;
        var componentPrefix = feature.ComponentPrefix;

        // Add comment for the feature
        registrations.Add($"            // {componentPrefix}");

        if (feature.HasListing)
        {
            registrations.Add($"            services.AddScoped<ServiceContracts.Features.{moduleNamespace}.I{componentPrefix}ListingDataService, Features.{moduleNamespace}.{componentPrefix}ListingServerDataService>();");
        }

        if (feature.HasForm)
        {
            registrations.Add($"            services.AddScoped<ServiceContracts.Features.{moduleNamespace}.I{componentPrefix}FormDataService, Features.{moduleNamespace}.{componentPrefix}FormServerDataService>();");
        }

        if (feature.HasSelectList)
        {
            registrations.Add($"            services.AddScoped<ServiceContracts.Features.{moduleNamespace}.I{componentPrefix}SelectListDataService, Features.{moduleNamespace}.{componentPrefix}SelectListServerDataService>();");
        }

        return registrations;
    }

    private List<string> GenerateClientSideRegistrations(Feature feature)
    {
        var registrations = new List<string>();
        var moduleNamespace = feature.ModuleNamespace;
        var componentPrefix = feature.ComponentPrefix;

        // Add comment for the feature
        registrations.Add($"            // {componentPrefix}");

        if (feature.HasListing)
        {
            registrations.Add($"            services.AddScoped<ServiceContracts.Features.{moduleNamespace}.I{componentPrefix}ListingDataService, Features.{moduleNamespace}.{componentPrefix}ListingClientDataService>();");
        }

        if (feature.HasForm)
        {
            registrations.Add($"            services.AddScoped<ServiceContracts.Features.{moduleNamespace}.I{componentPrefix}FormDataService, Features.{moduleNamespace}.{componentPrefix}FormClientDataService>();");
        }

        if (feature.HasSelectList)
        {
            registrations.Add($"            services.AddScoped<ServiceContracts.Features.{moduleNamespace}.I{componentPrefix}SelectListDataService, Features.{moduleNamespace}.{componentPrefix}SelectListClientDataService>();");
        }

        return registrations;
    }

    private async Task UpdateRegistrationFileWithPlaceholderAsync(string filePath, List<string> newRegistrations, string placeholder)
    {
        try
        {
            var content = await File.ReadAllTextAsync(filePath);

            if (!content.Contains(placeholder))
            {
                _logger.LogWarning("Placeholder {Placeholder} not found in file: {FilePath}", placeholder, filePath);
                return;
            }

            // Build the replacement text: new registrations + placeholder for next time
            var replacementText = string.Join(Environment.NewLine, newRegistrations) + Environment.NewLine +
                                 Environment.NewLine + "            " + placeholder;

            // Find the placeholder with its current indentation and replace it
            var lines = content.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == placeholder)
                {
                    // Get the current indentation
                    var indentation = lines[i].Substring(0, lines[i].IndexOf(placeholder));

                    // Replace with new registrations maintaining indentation
                    var indentedReplacements = newRegistrations.Select(reg =>
                        reg.StartsWith("            ") ? reg : indentation + reg.TrimStart()).ToList();

                    // Build the final replacement with proper indentation
                    var finalReplacement = string.Join(Environment.NewLine, indentedReplacements) +
                                         Environment.NewLine + Environment.NewLine + indentation + placeholder;

                    lines[i] = finalReplacement;
                    break;
                }
            }

            var updatedContent = string.Join(Environment.NewLine, lines);
            await File.WriteAllTextAsync(filePath, updatedContent);

            _logger.LogInformation("Updated registration file: {FilePath} with {Count} registrations",
                filePath, newRegistrations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update registration file: {FilePath}", filePath);
            throw;
        }
    }
}
