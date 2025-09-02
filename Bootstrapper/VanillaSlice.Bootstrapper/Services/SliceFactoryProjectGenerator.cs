using VanillaSlice.Bootstrapper.Models;
using Microsoft.Extensions.Logging;

namespace VanillaSlice.Bootstrapper.Services
{
    public class SliceFactoryProjectGenerator
    {
        private readonly TemplateEngineService _templateEngine;
        private readonly ILogger<SliceFactoryProjectGenerator> _logger;

        public SliceFactoryProjectGenerator(TemplateEngineService templateEngine, ILogger<SliceFactoryProjectGenerator> logger)
        {
            _templateEngine = templateEngine;
            _logger = logger;
        }

        public async Task<List<GeneratedFile>> GenerateSliceFactoryProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🏗️ Generating SliceFactory project using templates...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["RootNamespace"] = config.RootNamespace,
                    ["TargetFramework"] = "net9.0"
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "SliceFactory",
                    parameters,
                    $"{config.ProjectName}.SliceFactory");

                _logger.LogInformation("✅ Generated {FileCount} SliceFactory files", generatedFiles.Count);
                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate SliceFactory project");
                throw;
            }
        }
    }
}
