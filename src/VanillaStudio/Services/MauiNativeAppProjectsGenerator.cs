using ZKnow.VanillaStudio.Models;

namespace ZKnow.VanillaStudio.Services
{
    public class MauiNativeAppProjectsGenerator
    {
        private readonly TemplateEngineService _templateEngine;
        private readonly ILogger<MauiNativeAppProjectsGenerator> _logger;

        public MauiNativeAppProjectsGenerator(TemplateEngineService templateEngine, ILogger<MauiNativeAppProjectsGenerator> logger)
        {
            _templateEngine = templateEngine;
            _logger = logger;
        }

        public async Task<List<GeneratedFile>> GenerateMauiNativeAppProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("📱 Generating MAUI Native App project...");

                var parameters = new Dictionary<string, object>
                {
                    ["ProjectName"] = config.ProjectName,
                    ["ProjectNameLower"] = config.ProjectName.ToLower().Replace('.', '_'),
                    ["RootNamespace"] = $"{config.ProjectName}.MauiNativeApp",
                    ["TargetFramework"] = "net9.0",
                    ["NavigationType"] = config.MauiNavigationType.ToString()
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "MauiNativeApp",
                    parameters,
                    $"{config.ProjectName}.MauiNativeApp");

                _logger.LogInformation("✅ Generated {FileCount} MAUI Native App files with {NavigationType} navigation",
                    generatedFiles.Count, config.MauiNavigationType);

                return generatedFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate MAUI Native App project");
                throw;
            }
        }
    }
}