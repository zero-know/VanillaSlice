using ZKnow.VanillaStudio.Models;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace ZKnow.VanillaStudio.Services
{
    public class ProjectGenerationService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProjectGenerationService> _logger;
        private readonly ProjectTemplateService _templateService;

        public ProjectGenerationService(IWebHostEnvironment environment, ILogger<ProjectGenerationService> logger)
        {
            _environment = environment;
            _logger = logger;
            _templateService = new ProjectTemplateService();
        }

        public async Task<ProjectGenerationResult> GenerateProjectAsync(ProjectConfiguration config)
        {
            var result = new ProjectGenerationResult();
            
            try
            {
                _logger.LogInformation("Starting project generation for {ProjectName}", config.ProjectName);

                // Validate configuration
                var validationErrors = ValidateConfiguration(config);
                if (validationErrors.Any())
                {
                    result.Errors.AddRange(validationErrors);
                    result.Success = false;
                    result.Message = "Configuration validation failed";
                    return result;
                }

                // Generate project structure
                var projectStructure = await GenerateProjectStructureAsync(config);
                result.GeneratedFiles.AddRange(projectStructure);

                // Create ZIP file in memory for download
                var (zipData, zipFileName) = await CreateProjectZipAsync(config, projectStructure);
                result.ZipData = zipData;
                result.ZipFileName = zipFileName;
                result.DownloadUrl = null; // No longer needed for file-based downloads
                result.ProjectPath = null; // No longer needed for file-based downloads

                result.Success = true;
                result.Message = $"Project '{config.ProjectName}' generated successfully with {projectStructure.Count} files";

                _logger.LogInformation("Project generation completed successfully for {ProjectName}", config.ProjectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating project {ProjectName}", config.ProjectName);
                result.Success = false;
                result.Message = "An error occurred during project generation";
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        private List<string> ValidateConfiguration(ProjectConfiguration config)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(config.ProjectName))
                errors.Add("Project name is required");

            if (string.IsNullOrWhiteSpace(config.RootNamespace))
                errors.Add("Root namespace is required");

            if (string.IsNullOrWhiteSpace(config.OutputDirectory))
                errors.Add("Output directory is required");

            // Validate project name format
            if (!string.IsNullOrWhiteSpace(config.ProjectName) && 
                !System.Text.RegularExpressions.Regex.IsMatch(config.ProjectName, @"^[a-zA-Z][a-zA-Z0-9._]*$"))
            {
                errors.Add("Project name must start with a letter and contain only letters, numbers, dots, and underscores");
            }

            // Validate namespace format
            if (!string.IsNullOrWhiteSpace(config.RootNamespace) && 
                !System.Text.RegularExpressions.Regex.IsMatch(config.RootNamespace, @"^[a-zA-Z][a-zA-Z0-9._]*$"))
            {
                errors.Add("Root namespace must start with a letter and contain only letters, numbers, dots, and underscores");
            }

            return errors;
        }

        private async Task<List<GeneratedFile>> GenerateProjectStructureAsync(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // Generate solution file
            files.Add(GenerateSolutionFile(config));

            // Generate Framework.Core project
            files.AddRange(GenerateFrameworkCoreProject(config));

            // Generate Platform projects
            files.AddRange(GeneratePlatformProjects(config));

            // Generate main project structure based on configuration
            files.AddRange(await GenerateMainProjectStructureAsync(config));

            // Generate configuration files
            files.AddRange(GenerateConfigurationFiles(config));

            return files;
        }

        private GeneratedFile GenerateSolutionFile(ProjectConfiguration config)
        {
            var solutionContent = GenerateSolutionContent(config);
            
            return new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.sln",
                Content = solutionContent,
                Type = FileType.SolutionFile
            };
        }

        private string GenerateSolutionContent(ProjectConfiguration config)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            sb.AppendLine("# Visual Studio Version 17");
            sb.AppendLine("VisualStudioVersion = 17.6.33723.286");
            sb.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

            // Add project references based on configuration
            var projectGuid = Guid.NewGuid().ToString().ToUpper();
            
            // Framework folder
            sb.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"Framework\", \"Framework\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
            sb.AppendLine("EndProject");

            // Framework.Core project
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"Framework.Core\", \"Framework\\Framework.Core\\Framework.Core.csproj\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
            sb.AppendLine("EndProject");

            // Platform folder
            sb.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"Platform\", \"Platform\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
            sb.AppendLine("EndProject");

            // Platform.Common project
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"Platform.Common\", \"Platform\\Platform.Common\\Platform.Common.csproj\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
            sb.AppendLine("EndProject");

            if (config.IncludeDatabase)
            {
                sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"Platform.Server.Data\", \"Platform\\Platform.Server.Data\\Platform.Server.Data.csproj\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
                sb.AppendLine("EndProject");
            }

            // Main project folder
            sb.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"{config.ProjectName}\", \"{config.ProjectName}\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
            sb.AppendLine("EndProject");

            // WebPortal projects
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.WebPortal\", \"{config.ProjectName}\\WebPortal\\{config.ProjectName}.WebPortal\\{config.ProjectName}.WebPortal.csproj\", \"{{{projectGuid}}}\"");
            sb.AppendLine("EndProject");

            if (config.RenderingMode == RenderingMode.Auto)
            {
                sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.WebPortal.Client\", \"{config.ProjectName}\\WebPortal\\{config.ProjectName}.WebPortal.Client\\{config.ProjectName}.WebPortal.Client.csproj\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
                sb.AppendLine("EndProject");
            }

            if (config.ComponentStrategy == ComponentStrategy.CommonLibrary)
            {
                sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.WebPortal.Razor\", \"{config.ProjectName}\\WebPortal\\{config.ProjectName}.WebPortal.Razor\\{config.ProjectName}.WebPortal.Razor.csproj\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
                sb.AppendLine("EndProject");
            }

            if (config.PlatformType == PlatformType.WebAndMaui)
            {
                sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.MauiApp\", \"{config.ProjectName}\\MobileApp\\{config.ProjectName}.MauiApp\\{config.ProjectName}.MauiApp.csproj\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
                sb.AppendLine("EndProject");
            }

            if (config.UseAspireOrchestration)
            {
                sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.AppHost\", \"{config.ProjectName}\\{config.ProjectName}.AppHost\\{config.ProjectName}.AppHost.csproj\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
                sb.AppendLine("EndProject");
                sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.ServiceDefaults\", \"{config.ProjectName}\\{config.ProjectName}.ServiceDefaults\\{config.ProjectName}.ServiceDefaults.csproj\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
                sb.AppendLine("EndProject");
            }

            // Global sections
            sb.AppendLine("Global");
            sb.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            sb.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
            sb.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
            sb.AppendLine("\tEndGlobalSection");
            sb.AppendLine("EndGlobal");

            return sb.ToString();
        }

        private List<GeneratedFile> GenerateFrameworkCoreProject(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // Framework.Core.csproj
            files.Add(new GeneratedFile
            {
                RelativePath = "Framework/Framework.Core/Framework.Core.csproj",
                Content = GenerateFrameworkCoreProjectFile(config),
                Type = FileType.ProjectFile
            });

            // Add essential Framework.Core classes
            files.AddRange(GenerateFrameworkCoreClasses(config));

            return files;
        }

        private string GenerateFrameworkCoreProjectFile(ProjectConfiguration config)
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk.Razor"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <SupportedPlatform Include=""browser"" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.Web"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.Extensions.DependencyInjection.Abstractions"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.Extensions.Logging.Abstractions"" Version=""9.0.8"" />
    <PackageReference Include=""Newtonsoft.Json"" Version=""13.0.3"" />
  </ItemGroup>

</Project>";
        }

        private List<GeneratedFile> GenerateFrameworkCoreClasses(ProjectConfiguration config)
        {
            // This will be implemented to copy essential Framework.Core classes
            // For now, return empty list - we'll implement this in the next step
            return new List<GeneratedFile>();
        }

        private List<GeneratedFile> GeneratePlatformProjects(ProjectConfiguration config)
        {
            // This will generate Platform.Common and Platform.Server.Data projects
            // For now, return empty list - we'll implement this in the next step
            return new List<GeneratedFile>();
        }

        private async Task<List<GeneratedFile>> GenerateMainProjectStructureAsync(ProjectConfiguration config)
        {
            // This will generate the main project structure based on configuration
            // For now, return empty list - we'll implement this in the next step
            return new List<GeneratedFile>();
        }

        private List<GeneratedFile> GenerateConfigurationFiles(ProjectConfiguration config)
        {
            // This will generate appsettings.json, launchSettings.json, etc.
            // For now, return empty list - we'll implement this in the next step
            return new List<GeneratedFile>();
        }

        private async Task<(byte[] zipData, string zipFileName)> CreateProjectZipAsync(ProjectConfiguration config, List<GeneratedFile> files)
        {
            var zipFileName = $"{config.ProjectName}_{DateTime.Now:yyyyMMdd_HHmmss}.zip";

            using (var memoryStream = new MemoryStream())
            {
                using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var entry = zip.CreateEntry(file.RelativePath);
                        using (var entryStream = entry.Open())
                        using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                        {
                            await writer.WriteAsync(file.Content);
                        }
                    }
                }

                var zipData = memoryStream.ToArray();
                _logger.LogInformation("✅ ZIP file created in memory successfully: {ZipFileName} with {FileCount} files ({ZipSize} bytes)",
                    zipFileName, files.Count, zipData.Length);

                return (zipData, zipFileName);
            }
        }
    }
}
