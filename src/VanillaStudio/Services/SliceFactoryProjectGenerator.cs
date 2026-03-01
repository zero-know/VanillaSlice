using ZKnow.VanillaStudio.Models;
using Microsoft.Extensions.Logging;

namespace ZKnow.VanillaStudio.Services
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
                    ["TargetFramework"] = config.TargetFramework,
                    ["AspNetCoreVersion"] = config.AspNetCoreVersion,
                    ["UIFramework"] = config.UIFramework.ToString()
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
