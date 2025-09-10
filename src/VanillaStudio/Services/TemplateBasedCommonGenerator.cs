using ZKnow.VanillaStudio.Models;

namespace ZKnow.VanillaStudio.Services
{
    public class TemplateBasedCommonGenerator
    {
        private readonly TemplateEngineService _templateEngine;
        private readonly ILogger<TemplateBasedCommonGenerator> _logger;

        public TemplateBasedCommonGenerator(TemplateEngineService templateEngine, ILogger<TemplateBasedCommonGenerator> logger)
        {
            _templateEngine = templateEngine;
            _logger = logger;
        }

        public async Task<List<GeneratedFile>> GenerateCommonProjectAsync(ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("🏗️ Generating Common project using templates...");

                var parameters = new Dictionary<string, object>
                {
                    ["RootNamespace"] = $"{config.ProjectName}.Common",
                    ["ProjectName"] = config.ProjectName,
                    ["TargetFramework"] = "net9.0",
                    ["IncludeSampleData"] = config.IncludeSampleData
                };

                var generatedFiles = await _templateEngine.GenerateFromTemplateAsync(
                    "Common", 
                    parameters, 
                    $"{config.ProjectName}.Common/{config.ProjectName}.Common");

                // Filter files based on configuration
                var filteredFiles = FilterFilesByConfiguration(generatedFiles, config);

                _logger.LogInformation("✅ Generated {Count} Common files using templates", filteredFiles.Count);
                return filteredFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to generate Common project using templates");
                throw;
            }
        }

        private List<GeneratedFile> FilterFilesByConfiguration(List<GeneratedFile> files, ProjectConfiguration config)
        {
            var filteredFiles = new List<GeneratedFile>();

            foreach (var file in files)
            {
                // Always include project file and readme
                if (file.RelativePath.EndsWith(".csproj") || 
                    file.RelativePath.EndsWith("readme.md"))
                {
                    filteredFiles.Add(file);
                    continue;
                }

                // Include enum files only if sample data is enabled
                if (file.RelativePath.Contains("Enums/"))
                {
                    if (config.IncludeSampleData)
                    {
                        filteredFiles.Add(file);
                    }
                    continue;
                }

                // Include all other files
                filteredFiles.Add(file);
            }

            return filteredFiles;
        }
    }
}
