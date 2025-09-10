using ZKnow.VanillaStudio.Models;
using Microsoft.Extensions.Logging;

namespace ZKnow.VanillaStudio.Services
{
    public class TemplateBasedFrameworkCoreGenerator
    {
        private readonly TemplateEngineService _templateEngine;
        private readonly ILogger<TemplateBasedFrameworkCoreGenerator> _logger;

        public TemplateBasedFrameworkCoreGenerator(
            TemplateEngineService templateEngine,
            ILogger<TemplateBasedFrameworkCoreGenerator> logger)
        {
            _templateEngine = templateEngine;
            _logger = logger;
        }

        public async Task<List<GeneratedFile>> GenerateCompleteFrameworkCoreProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🏗️ Generating Framework.Core project using templates...");

                var parameters = new Dictionary<string, object>
                {
                    ["RootNamespace"] = $"{config.ProjectName}.Framework",
                    ["ProjectName"] = config.ProjectName,
                    ["TargetFramework"] = "net9.0",
                    ["IncludeDatabase"] = config.IncludeDatabase,
                    ["IncludeAuthentication"] = config.IncludeAuthentication
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "FrameworkCore",
                    parameters,
                    $"{config.ProjectName}.Base/{config.ProjectName}.Framework");

                _logger.LogInformation("✅ Generated {FileCount} Framework.Core files using templates", generatedFiles.Count);

                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate Framework.Core project using templates");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateBaseTypesAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("📋 Generating base types using templates...");

                var parameters = new Dictionary<string, object>
                {
                    ["RootNamespace"] = $"{config.ProjectName}.Framework",
                    ["ProjectName"] = config.ProjectName
                };

                // Filter to only base type files
                var allFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "FrameworkCore",
                    parameters,
                    $"{config.ProjectName}.Base/{config.ProjectName}.Framework");

                var baseTypeFiles = allFiles.Where(f =>
                    f.RelativePath.Contains("BaseTypes/") ||
                    f.RelativePath.EndsWith($"{config.ProjectName}.Framework.csproj"))
                    .ToList();

                _logger.LogInformation("✅ Generated {FileCount} base type files using templates", baseTypeFiles.Count);

                return baseTypeFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate base types using templates");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateInterfacesAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🔌 Generating interfaces using templates...");

                var parameters = new Dictionary<string, object>
                {
                    ["RootNamespace"] = $"{config.ProjectName}.Framework",
                    ["ProjectName"] = config.ProjectName
                };

                // Filter to only interface files
                var allFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "FrameworkCore",
                    parameters,
                    $"{config.ProjectName}.Base/{config.ProjectName}.Framework");

                var interfaceFiles = allFiles.Where(f => f.RelativePath.Contains("Interfaces/"))
                    .ToList();

                _logger.LogInformation("✅ Generated {FileCount} interface files using templates", interfaceFiles.Count);

                return interfaceFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate interfaces using templates");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateExtensionsAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🔧 Generating extensions using templates...");

                var parameters = new Dictionary<string, object>
                {
                    ["RootNamespace"] = $"{config.ProjectName}.Framework",
                    ["ProjectName"] = config.ProjectName
                };

                // Filter to only extension files
                var allFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "FrameworkCore",
                    parameters,
                    $"{config.ProjectName}.Base/{config.ProjectName}.Framework");

                var extensionFiles = allFiles.Where(f => f.RelativePath.Contains("Extensions/"))
                    .ToList();

                _logger.LogInformation("✅ Generated {FileCount} extension files using templates", extensionFiles.Count);

                return extensionFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate extensions using templates");
                throw;
            }
        }

        public async Task<List<GeneratedFile>> GenerateUtilsAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🛠️ Generating utils using templates...");

                var parameters = new Dictionary<string, object>
                {
                    ["RootNamespace"] = $"{config.ProjectName}.Framework",
                    ["ProjectName"] = config.ProjectName
                };

                // Filter to only util files
                var allFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "FrameworkCore",
                    parameters,
                    $"{config.ProjectName}.Base/{config.ProjectName}.Framework");

                var utilFiles = allFiles.Where(f => f.RelativePath.Contains("Utils/"))
                    .ToList();

                _logger.LogInformation("✅ Generated {FileCount} util files using templates", utilFiles.Count);

                return utilFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate utils using templates");
                throw;
            }
        }
    }
}
