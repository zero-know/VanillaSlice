using VanillaSlice.Bootstrapper.Models;

namespace VanillaSlice.Bootstrapper.Services
{
    public class InfrastructureProjectsGenerator
    {
        private readonly ILogger<InfrastructureProjectsGenerator> _logger;
        private readonly TemplateEngineService _templateEngine;

        public InfrastructureProjectsGenerator(ILogger<InfrastructureProjectsGenerator> logger, TemplateEngineService templateEngine)
        {
            _logger = logger;
            _templateEngine = templateEngine;
        }

        public async Task<List<GeneratedFile>> GenerateAllInfrastructureProjectsAsync(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            try
            {
                _logger.LogInformation("🏗️ Generating Infrastructure projects...");

                // Generate WebAPI project
                files.AddRange(await GenerateWebAPIProjectAsync(config));

                // Generate ServiceDefaults project
                files.AddRange(await GenerateServiceDefaultsProjectAsync(config));

                // Generate AppHost project (if Aspire is enabled)
                if (config.UseAspireOrchestration)
                {
                    files.AddRange(await GenerateAppHostProjectAsync(config));
                }

                _logger.LogInformation("✅ Successfully generated {FileCount} Infrastructure project files", files.Count);
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate Infrastructure projects");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateWebAPIProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🌐 Generating WebAPI project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["RootNamespace"] = $"{config.ProjectName}.WebAPI",
                    ["TargetFramework"] = "net9.0"
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "WebAPI",
                    parameters,
                    $"{config.ProjectName}.WebAPI");

                _logger.LogInformation("✅ Generated {FileCount} WebAPI files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate WebAPI project");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateServiceDefaultsProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("⚙️ Generating ServiceDefaults project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["RootNamespace"] = $"{config.ProjectName}.ServiceDefaults",
                    ["TargetFramework"] = "net9.0"
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "ServiceDefaults",
                    parameters,
                    $"{config.ProjectName}.ServiceDefaults");

                _logger.LogInformation("✅ Generated {FileCount} ServiceDefaults files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate ServiceDefaults project");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateAppHostProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🚀 Generating AppHost project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["ProjectNameUnderScored"] = config.ProjectName.Replace('.','_'),
                    ["ProjectNameHyphened"] = config.ProjectName.Replace('.','-'),
                    ["RootNamespace"] = $"{config.ProjectName}.AppHost",
                    ["TargetFramework"] = "net9.0",
                    ["UserSecretsId"] = Guid.NewGuid().ToString()
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "AppHost",
                    parameters,
                    $"{config.ProjectName}.AppHost");

                _logger.LogInformation("✅ Generated {FileCount} AppHost files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate AppHost project");
                throw;
            }
        }
    }
}
