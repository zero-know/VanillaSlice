using VanillaSlice.Bootstrapper.Models;

namespace VanillaSlice.Bootstrapper.Services
{
    public class WebPortalProjectsGenerator
    {
        private readonly ILogger<WebPortalProjectsGenerator> _logger;
        private readonly TemplateEngineService _templateEngine;

        public WebPortalProjectsGenerator(ILogger<WebPortalProjectsGenerator> logger, TemplateEngineService templateEngine)
        {
            _logger = logger;
            _templateEngine = templateEngine;
        }

        public async Task<List<GeneratedFile>> GenerateAllWebPortalProjectsAsync(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            try
            {
                _logger.LogInformation("🌐 Generating WebPortal projects...");

                // Generate main WebPortal project
                files.AddRange(await GenerateWebPortalProjectAsync(config));

                // Generate WebPortal.Client project (for Auto rendering mode)
                if (config.RenderingMode == RenderingMode.Auto)
                {
                    files.AddRange(await GenerateWebPortalClientProjectAsync(config));
                }

                _logger.LogInformation("✅ Successfully generated {FileCount} WebPortal project files", files.Count);
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate WebPortal projects");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateWebPortalProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🏠 Generating WebPortal main project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["RootNamespace"] = $"{config.ProjectName}.WebPortal",
                    ["TargetFramework"] = "net9.0",
                    ["RenderingMode"] = config.RenderingMode.ToString(),
                    ["IncludeAuthentication"] = config.IncludeAuthentication,
                    ["UserSecretsId"] = Guid.NewGuid().ToString()
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "WebPortal",
                    parameters,
                    $"{config.ProjectName}.WebPortal/{config.ProjectName}.WebPortal");

                _logger.LogInformation("✅ Generated {FileCount} WebPortal main project files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate WebPortal main project");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateWebPortalClientProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("📱 Generating WebPortal.Client project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["RootNamespace"] = $"{config.ProjectName}.WebPortal.Client",
                    ["TargetFramework"] = "net9.0"
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "WebPortalClient",
                    parameters,
                    $"{config.ProjectName}.WebPortal/{config.ProjectName}.WebPortal.Client");

                _logger.LogInformation("✅ Generated {FileCount} WebPortal.Client project files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate WebPortal.Client project");
                throw;
            }
        }
    }
}
