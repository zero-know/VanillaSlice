using VanillaSlice.Bootstrapper.Models;

namespace VanillaSlice.Bootstrapper.Services
{
    public class PlatformProjectsGenerator
    {
        private readonly ILogger<PlatformProjectsGenerator> _logger;
        private readonly TemplateEngineService _templateEngine;

        public PlatformProjectsGenerator(ILogger<PlatformProjectsGenerator> logger, TemplateEngineService templateEngine)
        {
            _logger = logger;
            _templateEngine = templateEngine;
        }

        public async Task<List<GeneratedFile>> GenerateAllPlatformProjectsAsync(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            try
            {
                _logger.LogInformation("🏗️ Generating Platform projects...");

                // Generate ServiceContracts project
                files.AddRange(await GenerateServiceContractsProjectAsync(config));

                // Generate Server.DataServices project
                files.AddRange(await GenerateServerDataServicesProjectAsync(config));

                // Generate Client.Shared project
                files.AddRange(await GenerateClientSharedProjectAsync(config));

                // Generate Razor library project
                files.AddRange(await GenerateRazorLibraryProjectAsync(config));

                _logger.LogInformation("✅ Successfully generated {FileCount} Platform project files", files.Count);
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate Platform projects");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateServiceContractsProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("📋 Generating ServiceContracts project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["RootNamespace"] = $"{config.ProjectName}.ServiceContracts",
                    ["TargetFramework"] = "net9.0"
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "ServiceContracts",
                    parameters,
                    $"{config.ProjectName}.Platform/{config.ProjectName}.ServiceContracts");

                _logger.LogInformation("✅ Generated {FileCount} ServiceContracts files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate ServiceContracts project");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateServerDataServicesProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🔧 Generating Server.DataServices project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["RootNamespace"] = $"{config.ProjectName}.Server.DataServices",
                    ["TargetFramework"] = "net9.0"
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "ServerDataServices",
                    parameters,
                    $"{config.ProjectName}.Platform/{config.ProjectName}.Server.DataServices");

                _logger.LogInformation("✅ Generated {FileCount} Server.DataServices files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate Server.DataServices project");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateClientSharedProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🌐 Generating Client.Shared project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["RootNamespace"] = $"{config.ProjectName}.Client.Shared",
                    ["TargetFramework"] = "net9.0"
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "ClientShared",
                    parameters,
                    $"{config.ProjectName}.Platform/{config.ProjectName}.Client.Shared");

                _logger.LogInformation("✅ Generated {FileCount} Client.Shared files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate Client.Shared project");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateRazorLibraryProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🎨 Generating Razor library project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["RootNamespace"] = $"{config.ProjectName}.Razor",
                    ["TargetFramework"] = "net9.0"
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "RazorLibrary",
                    parameters,
                    $"{config.ProjectName}.Platform/{config.ProjectName}.Razor");

                _logger.LogInformation("✅ Generated {FileCount} Razor library files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate Razor library project");
                throw;
            }
        }
    }
}
