using System.IO.Compression;
using System.Text;
using System.Text.Json;
using ZKnow.VanillaStudio.Models;
using ZKnow.VanillaStudio.Services;

namespace ZKnow.VanillaStudio.Services
{
    public class EnhancedProjectGenerationService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EnhancedProjectGenerationService> _logger;
        private readonly ProjectTemplateService _templateService;
        private readonly TemplateEngineService _templateEngine;
        private readonly TemplateBasedFrameworkCoreGenerator _templateBasedFrameworkCoreGenerator;
        private readonly TemplateBasedServerDataGenerator _templateBasedServerDataGenerator;
        private readonly TemplateBasedCommonGenerator _templateBasedCommonGenerator;
        private readonly PlatformProjectsGenerator _platformProjectsGenerator;
        private readonly InfrastructureProjectsGenerator _infrastructureProjectsGenerator;
        private readonly WebPortalProjectsGenerator _webPortalProjectsGenerator;
        private readonly HybridAppProjectsGenerator _hybridAppProjectsGenerator;
        private readonly ProjectValidationService _validationService;

        public EnhancedProjectGenerationService(
            IWebHostEnvironment environment,
            ILogger<EnhancedProjectGenerationService> logger,
            TemplateEngineService templateEngine,
            TemplateBasedFrameworkCoreGenerator templateBasedFrameworkCoreGenerator,
            TemplateBasedServerDataGenerator templateBasedServerDataGenerator,
            TemplateBasedCommonGenerator templateBasedCommonGenerator,
            PlatformProjectsGenerator platformProjectsGenerator,
            InfrastructureProjectsGenerator infrastructureProjectsGenerator,
            WebPortalProjectsGenerator webPortalProjectsGenerator,
            HybridAppProjectsGenerator hybridAppProjectsGenerator,
            ProjectValidationService validationService)
        {
            _environment = environment;
            _logger = logger;
            _templateEngine = templateEngine;
            _templateService = new ProjectTemplateService();
            _templateBasedFrameworkCoreGenerator = templateBasedFrameworkCoreGenerator;
            _templateBasedServerDataGenerator = templateBasedServerDataGenerator;
            _templateBasedCommonGenerator = templateBasedCommonGenerator;
            _platformProjectsGenerator = platformProjectsGenerator;
            _infrastructureProjectsGenerator = infrastructureProjectsGenerator;
            _webPortalProjectsGenerator = webPortalProjectsGenerator;
            _hybridAppProjectsGenerator = hybridAppProjectsGenerator;
            _validationService = validationService;
        }

        public async Task<ProjectGenerationResult> GenerateProjectAsync(ProjectConfiguration config)
        {
            var result = new ProjectGenerationResult();

            try
            {
                _logger.LogInformation("🚀 ENHANCED PROJECT GENERATION STARTED for {ProjectName}", config.ProjectName);

                // Validate configuration
                var validationErrors = ValidateConfiguration(config);
                if (validationErrors.Any())
                {
                    result.Errors.AddRange(validationErrors);
                    result.Success = false;
                    result.Message = "Configuration validation failed";
                    return result;
                }

                // Generate complete project structure
                var projectStructure = await GenerateCompleteProjectStructureAsync(config);
                result.GeneratedFiles.AddRange(projectStructure);

                // Create ZIP file in memory for download
                var (zipData, zipFileName) = await CreateProjectZipAsync(config, projectStructure);
                result.ZipData = zipData;
                result.ZipFileName = zipFileName;
                result.DownloadUrl = null; // No longer needed for file-based downloads
                result.ProjectPath = null; // No longer needed for file-based downloads

                result.Success = true;
                result.Message = $"Complete project '{config.ProjectName}' generated successfully with {projectStructure.Count} files including SliceFactory and sample CRUD";

                _logger.LogInformation("✅ ENHANCED PROJECT GENERATION COMPLETED successfully for {ProjectName} with {FileCount} files", config.ProjectName, projectStructure.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating enhanced project {ProjectName}", config.ProjectName);
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

        private async Task<List<GeneratedFile>> GenerateCompleteProjectStructureAsync(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // Generate solution file
            _logger.LogInformation("📄 Generating solution file...");
            files.Add(GenerateSolutionFile(config));
            _logger.LogInformation("✅ Solution file generated: {Count} files", 1);

            // Generate Framework.Core project with ALL essential classes using templates
            _logger.LogInformation("🏗️ Generating Framework.Core project using templates...");
            var frameworkFiles = await GenerateCompleteFrameworkCoreProjectAsync(config);
            files.AddRange(frameworkFiles);
            _logger.LogInformation("✅ Framework.Core generated using templates: {Count} files", frameworkFiles.Count);

            // Generate Common project using templates
            _logger.LogInformation("📚 Generating Common project using templates...");
            var commonFiles = await _templateBasedCommonGenerator.GenerateCommonProjectAsync(config);
            files.AddRange(commonFiles);
            _logger.LogInformation("✅ Common project generated using templates: {Count} files", commonFiles.Count);

            // Generate Server.Data project using templates
            _logger.LogInformation("🗄️ Generating Server.Data project using templates...");
            var serverDataFiles = await _templateBasedServerDataGenerator.GenerateServerDataProjectAsync(config);
            files.AddRange(serverDataFiles);
            _logger.LogInformation("✅ Server.Data project generated using templates: {Count} files", serverDataFiles.Count);

            // Generate Platform projects
            _logger.LogInformation("🏢 Generating Platform projects...");
            var platformFiles = await _platformProjectsGenerator.GenerateAllPlatformProjectsAsync(config);
            files.AddRange(platformFiles);
            _logger.LogInformation("✅ Platform projects generated: {Count} files", platformFiles.Count);

            // Generate Infrastructure projects
            _logger.LogInformation("🏗️ Generating Infrastructure projects...");
            var infrastructureFiles = await _infrastructureProjectsGenerator.GenerateAllInfrastructureProjectsAsync(config);
            files.AddRange(infrastructureFiles);
            _logger.LogInformation("✅ Infrastructure projects generated: {Count} files", infrastructureFiles.Count);

            // Generate SliceFactory project (COMPLETE)
            _logger.LogInformation("⚙️ Generating SliceFactory project...");
            var sliceFactoryFiles = await GenerateSliceFactoryProjectAsync(config);
            files.AddRange(sliceFactoryFiles);
            _logger.LogInformation("✅ SliceFactory project generated: {Count} files", sliceFactoryFiles.Count);

            // Generate WebPortal projects
            _logger.LogInformation("🏠 Generating WebPortal projects...");
            var webPortalFiles = await _webPortalProjectsGenerator.GenerateAllWebPortalProjectsAsync(config);
            files.AddRange(webPortalFiles);
            _logger.LogInformation("✅ WebPortal projects generated: {Count} files", webPortalFiles.Count);

            // Generate HybridApp project if MAUI is included
            if (config.IncludeHybridMaui)
            {
                _logger.LogInformation("📱 Generating HybridApp project...");
                var hybridAppFiles = await _hybridAppProjectsGenerator.GenerateHybridAppProjectAsync(config);
                files.AddRange(hybridAppFiles);
                _logger.LogInformation("✅ HybridApp project generated: {Count} files", hybridAppFiles.Count);
            }

            // Generate sample CRUD components with working example
            _logger.LogInformation("📋 Generating sample CRUD components...");
            var crudFiles = GenerateSampleCrudComponents(config);
            files.AddRange(crudFiles);
            _logger.LogInformation("✅ Sample CRUD components generated: {Count} files", crudFiles.Count);

            // Configuration files are now generated by the individual project templates
            // No need to generate them manually to avoid duplicates

            // Validate generated projects
            _logger.LogInformation("🔍 Validating generated projects...");
            var validationResult = await _validationService.ValidateGeneratedProjectsAsync(files, config);

            if (validationResult.IsValid)
            {
                _logger.LogInformation("✅ Project validation passed: {ValidatedCount} items validated", validationResult.ValidatedItems.Count);
            }
            else
            {
                _logger.LogWarning("⚠️ Project validation found {IssueCount} issues:", validationResult.Issues.Count);
                foreach (var issue in validationResult.Issues)
                {
                    _logger.LogWarning("  - {Issue}", issue);
                }
            }

            _logger.LogInformation("🎯 Total files generated: {TotalCount}", files.Count);
            return files;
        }

        private GeneratedFile GenerateSolutionFile(ProjectConfiguration config)
        {
            var solutionContent = GenerateCompleteSolutionContent(config);
            
            return new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.sln",
                Content = solutionContent,
                Type = FileType.SolutionFile
            };
        }

        private string GenerateCompleteSolutionContent(ProjectConfiguration config)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            sb.AppendLine("# Visual Studio Version 17");
            sb.AppendLine("VisualStudioVersion = 17.6.33723.286");
            sb.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

            // Generate GUIDs for all projects
            var baseFolderGuid = Guid.NewGuid().ToString().ToUpper();
            var platformFolderGuid = Guid.NewGuid().ToString().ToUpper();
            var commonFolderGuid = Guid.NewGuid().ToString().ToUpper();
            var webPortalFolderGuid = Guid.NewGuid().ToString().ToUpper();

            var frameworkGuid = Guid.NewGuid().ToString().ToUpper();
            var sourceGenGuid = Guid.NewGuid().ToString().ToUpper();
            var commonProjectGuid = Guid.NewGuid().ToString().ToUpper();
            var serverDataGuid = Guid.NewGuid().ToString().ToUpper();
            var serviceContractsGuid = Guid.NewGuid().ToString().ToUpper();
            var serverDataServicesGuid = Guid.NewGuid().ToString().ToUpper();
            var clientSharedGuid = Guid.NewGuid().ToString().ToUpper();
            var razorGuid = Guid.NewGuid().ToString().ToUpper();
            var webPortalGuid = Guid.NewGuid().ToString().ToUpper();
            var webPortalClientGuid = Guid.NewGuid().ToString().ToUpper();
            var webAPIGuid = Guid.NewGuid().ToString().ToUpper();
            var serviceDefaultsGuid = Guid.NewGuid().ToString().ToUpper();
            var appHostGuid = Guid.NewGuid().ToString().ToUpper();

            // Solution folders
            sb.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"{config.ProjectName}.Base\", \"{config.ProjectName}.Base\", \"{{{baseFolderGuid}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"{config.ProjectName}.Platform\", \"{config.ProjectName}.Platform\", \"{{{platformFolderGuid}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"{config.ProjectName}.Common\", \"{config.ProjectName}.Common\", \"{{{commonFolderGuid}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"{config.ProjectName}.WebPortal\", \"{config.ProjectName}.WebPortal\", \"{{{webPortalFolderGuid}}}\"");
            sb.AppendLine("EndProject");

            // Main projects
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.WebPortal\", \"{config.ProjectName}.WebPortal\\{config.ProjectName}.WebPortal\\{config.ProjectName}.WebPortal.csproj\", \"{{{webPortalGuid}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.WebPortal.Client\", \"{config.ProjectName}.WebPortal\\{config.ProjectName}.WebPortal.Client\\{config.ProjectName}.WebPortal.Client.csproj\", \"{{{webPortalClientGuid}}}\"");
            sb.AppendLine("EndProject");

            if (config.UseAspireOrchestration)
            {
                sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.AppHost\", \"{config.ProjectName}.AppHost\\{config.ProjectName}.AppHost.csproj\", \"{{{appHostGuid}}}\"");
                sb.AppendLine("EndProject");
            }

            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.ServiceDefaults\", \"{config.ProjectName}.ServiceDefaults\\{config.ProjectName}.ServiceDefaults.csproj\", \"{{{serviceDefaultsGuid}}}\"");
            sb.AppendLine("EndProject");

            // Base projects
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.Framework\", \"{config.ProjectName}.Base\\{config.ProjectName}.Framework\\{config.ProjectName}.Framework.csproj\", \"{{{frameworkGuid}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.SliceFactory\", \"{config.ProjectName}.SliceFactory\\{config.ProjectName}.SliceFactory.csproj\", \"{{{sourceGenGuid}}}\"");
            sb.AppendLine("EndProject");

            // Common projects
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.Server.Data\", \"{config.ProjectName}.Common\\{config.ProjectName}.Server.Data\\{config.ProjectName}.Server.Data.csproj\", \"{{{serverDataGuid}}}\"");
            sb.AppendLine("EndProject");

            // Platform projects
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.Client.Shared\", \"{config.ProjectName}.Platform\\{config.ProjectName}.Client.Shared\\{config.ProjectName}.Client.Shared.csproj\", \"{{{clientSharedGuid}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.Razor\", \"{config.ProjectName}.Platform\\{config.ProjectName}.Razor\\{config.ProjectName}.Razor.csproj\", \"{{{razorGuid}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.Server.DataServices\", \"{config.ProjectName}.Platform\\{config.ProjectName}.Server.DataServices\\{config.ProjectName}.Server.DataServices.csproj\", \"{{{serverDataServicesGuid}}}\"");
            sb.AppendLine("EndProject");
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.ServiceContracts\", \"{config.ProjectName}.Platform\\{config.ProjectName}.ServiceContracts\\{config.ProjectName}.ServiceContracts.csproj\", \"{{{serviceContractsGuid}}}\"");
            sb.AppendLine("EndProject");

            // WebAPI project
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.WebAPI\", \"{config.ProjectName}.WebAPI\\{config.ProjectName}.WebAPI.csproj\", \"{{{webAPIGuid}}}\"");
            sb.AppendLine("EndProject");

            // Add Common project
            sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.Common\", \"{config.ProjectName}.Common\\{config.ProjectName}.Common\\{config.ProjectName}.Common.csproj\", \"{{{commonProjectGuid}}}\"");
            sb.AppendLine("EndProject");

            if (config.IncludeHybridMaui)
            {
                sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{config.ProjectName}.HybridApp\", \"{config.ProjectName}.HybridApp\\{config.ProjectName}.HybridApp.csproj\", \"{{{Guid.NewGuid().ToString().ToUpper()}}}\"");
                sb.AppendLine("EndProject");
            }

            // Global sections
            sb.AppendLine("Global");
            sb.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            sb.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
            sb.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
            sb.AppendLine("\tEndGlobalSection");

            // Add project configuration platforms for each project
            sb.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");

            // Add configurations for all projects
            var allProjectGuids = new[] {
                webPortalGuid, webPortalClientGuid, frameworkGuid, sourceGenGuid,
                commonProjectGuid, serverDataGuid, serviceContractsGuid,
                serverDataServicesGuid, clientSharedGuid, razorGuid,
                webAPIGuid, serviceDefaultsGuid
            };

            if (config.UseAspireOrchestration)
            {
                allProjectGuids = allProjectGuids.Append(appHostGuid).ToArray();
            }

            foreach (var guid in allProjectGuids)
            {
                sb.AppendLine($"\t\t{{{guid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                sb.AppendLine($"\t\t{{{guid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                sb.AppendLine($"\t\t{{{guid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                sb.AppendLine($"\t\t{{{guid}}}.Release|Any CPU.Build.0 = Release|Any CPU");
            }

            sb.AppendLine("\tEndGlobalSection");

            // Add nested projects section
            sb.AppendLine("\tGlobalSection(NestedProjects) = preSolution");

            // Base folder projects
            sb.AppendLine($"\t\t{{{frameworkGuid}}} = {{{baseFolderGuid}}}");

            // Common folder projects
            sb.AppendLine($"\t\t{{{commonProjectGuid}}} = {{{commonFolderGuid}}}");
            sb.AppendLine($"\t\t{{{serverDataGuid}}} = {{{commonFolderGuid}}}");

            // Platform folder projects
            sb.AppendLine($"\t\t{{{serviceContractsGuid}}} = {{{platformFolderGuid}}}");
            sb.AppendLine($"\t\t{{{serverDataServicesGuid}}} = {{{platformFolderGuid}}}");
            sb.AppendLine($"\t\t{{{clientSharedGuid}}} = {{{platformFolderGuid}}}");
            sb.AppendLine($"\t\t{{{razorGuid}}} = {{{platformFolderGuid}}}");

            // WebPortal folder projects
            sb.AppendLine($"\t\t{{{webPortalGuid}}} = {{{webPortalFolderGuid}}}");
            sb.AppendLine($"\t\t{{{webPortalClientGuid}}} = {{{webPortalFolderGuid}}}");

            sb.AppendLine("\tEndGlobalSection");

            sb.AppendLine("\tGlobalSection(ExtensibilityGlobals) = postSolution");
            sb.AppendLine($"\t\tSolutionGuid = {{{Guid.NewGuid().ToString().ToUpper()}}}");
            sb.AppendLine("\tEndGlobalSection");
            sb.AppendLine("EndGlobal");

            return sb.ToString();
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

        private async Task<List<GeneratedFile>> GenerateCompleteFrameworkCoreProjectAsync(ProjectConfiguration config)
        {
            return await _templateBasedFrameworkCoreGenerator.GenerateCompleteFrameworkCoreProjectAsync(config);
        }

        private List<GeneratedFile> GenerateCompletePlatformProjects(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // Platform.Common project
            files.Add(new GeneratedFile
            {
                RelativePath = "Platform/Platform.Common/Platform.Common.csproj",
                Content = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>",
                Type = FileType.ProjectFile
            });

            // Platform.Common Constants
            files.Add(new GeneratedFile
            {
                RelativePath = "Platform/Platform.Common/Constants/PlatformConstants.cs",
                Content = @"namespace Platform.Common.Constants
{
    public static class PlatformConstants
    {
        public static int PartyId { get; set; }
        public const string AuthTokenKey = ""auth_token"";
        public const string RefreshTokenKey = ""refresh_token"";
    }
}",
                Type = FileType.CSharpCode
            });

            if (config.IncludeDatabase)
            {
                // Platform.Server.Data project
                files.Add(new GeneratedFile
                {
                    RelativePath = "Platform/Platform.Server.Data/Platform.Server.Data.csproj",
                    Content = GeneratePlatformServerDataProjectFile(config),
                    Type = FileType.ProjectFile
                });

                // DbContext
                files.Add(new GeneratedFile
                {
                    RelativePath = "Platform/Platform.Server.Data/EF/AppDbContext.cs",
                    Content = GenerateAppDbContextContent(config),
                    Type = FileType.CSharpCode
                });
            }

            return files;
        }

        private async Task<List<GeneratedFile>> GenerateSliceFactoryProjectAsync(ProjectConfiguration config)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var sliceFactoryLogger = loggerFactory.CreateLogger<SliceFactoryProjectGenerator>();
            var generator = new SliceFactoryProjectGenerator(_templateEngine, sliceFactoryLogger);
            return await generator.GenerateSliceFactoryProjectAsync(config);
        }

        private async Task<List<GeneratedFile>> GenerateMainProjectStructureAsync(ProjectConfiguration config)
        {
            await Task.CompletedTask;
            var files = _templateService.GenerateMainProjectStructure(config);

            // Add HTTP client services
            var httpGenerator = new HttpClientGenerator();
            files.AddRange(httpGenerator.GenerateHttpClientServices(config));

            return files;
        }

        private List<GeneratedFile> GenerateSampleCrudComponents(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // Sample CRUD components are now generated through the template system

            return files;
        }


        private string GeneratePlatformServerDataProjectFile(ProjectConfiguration config)
        {
            var dbProvider = config.DatabaseProvider switch
            {
                DatabaseProvider.SqlServer => "Microsoft.EntityFrameworkCore.SqlServer",
                DatabaseProvider.PostgreSQL => "Npgsql.EntityFrameworkCore.PostgreSQL",
                DatabaseProvider.SQLite => "Microsoft.EntityFrameworkCore.Sqlite",
                _ => "Microsoft.EntityFrameworkCore.SqlServer"
            };

            return $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.EntityFrameworkCore"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.Design"" Version=""9.0.8"" />
    <PackageReference Include=""{dbProvider}"" Version=""9.0.8"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""..\..\Framework\Framework.Core\Framework.Core.csproj"" />
  </ItemGroup>
</Project>";
        }

        private string GenerateAppDbContextContent(ProjectConfiguration config)
        {
            var connectionStringKey = config.DatabaseProvider switch
            {
                DatabaseProvider.SqlServer => "DefaultConnection",
                DatabaseProvider.PostgreSQL => "PostgreSQLConnection",
                DatabaseProvider.SQLite => "SQLiteConnection",
                _ => "DefaultConnection"
            };

            return $@"using Microsoft.EntityFrameworkCore;

namespace Platform.Server.Data.EF
{{
    public class AppDbContext : DbContext
    {{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {{
        }}

        // Sample DbSet - replace with your actual entities
        public DbSet<Product> Products {{ get; set; }}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {{
            base.OnModelCreating(modelBuilder);

            // Configure your entities here
            modelBuilder.Entity<Product>(entity =>
            {{
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType(""decimal(18,2)"");
            }});
        }}
    }}

    // Sample entity - replace with your actual entities
    public class Product
    {{
        public int Id {{ get; set; }}
        public string Name {{ get; set; }} = string.Empty;
        public string? Description {{ get; set; }}
        public decimal Price {{ get; set; }}
        public DateTime CreatedAt {{ get; set; }} = DateTime.UtcNow;
        public bool IsActive {{ get; set; }} = true;
    }}
}}";
        }



    }
}
