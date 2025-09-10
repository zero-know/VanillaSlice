using ZKnow.VanillaStudio.Models;

namespace ZKnow.VanillaStudio.Services
{
    public class HybridAppProjectsGenerator
    {
        private readonly TemplateEngineService _templateEngine;
        private readonly ILogger<HybridAppProjectsGenerator> _logger;

        public HybridAppProjectsGenerator(TemplateEngineService templateEngine, ILogger<HybridAppProjectsGenerator> logger)
        {
            _templateEngine = templateEngine;
            _logger = logger;
        }

        public async Task<List<GeneratedFile>> GenerateHybridAppProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("📱 Generating HybridApp project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["ProjectNameLower"] = config.ProjectName.ToLower().Replace('.', '_'),
                    ["RootNamespace"] = $"{config.ProjectName}.HybridApp",
                    ["TargetFramework"] = "net9.0"
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "HybridApp",
                    parameters,
                    $"{config.ProjectName}.HybridApp");

                _logger.LogInformation("✅ Generated {FileCount} HybridApp files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate HybridApp project");
                throw;
            }
        }
    }
}
