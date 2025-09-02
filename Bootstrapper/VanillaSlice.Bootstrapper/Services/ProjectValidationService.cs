using VanillaSlice.Bootstrapper.Models;

namespace VanillaSlice.Bootstrapper.Services
{
    public class ProjectValidationService
    {
        private readonly ILogger<ProjectValidationService> _logger;

        public ProjectValidationService(ILogger<ProjectValidationService> logger)
        {
            _logger = logger;
        }

        public Task<ValidationResult> ValidateGeneratedProjectsAsync(List<GeneratedFile> generatedFiles, ProjectConfiguration config)
        {
            var result = new ValidationResult();
            
            try
            {
                _logger.LogInformation("🔍 Starting project validation...");

                // Validate project structure
                ValidateProjectStructure(generatedFiles, config, result);

                // Validate critical files exist
                ValidateCriticalFiles(generatedFiles, config, result);

                // Validate project references
                ValidateProjectReferences(generatedFiles, config, result);

                // Validate UI components
                ValidateUIComponents(generatedFiles, config, result);

                _logger.LogInformation("✅ Project validation completed. Success: {IsValid}, Issues: {IssueCount}",
                    result.IsValid, result.Issues.Count);

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Project validation failed");
                result.Issues.Add($"Validation failed with exception: {ex.Message}");
                return Task.FromResult(result);
            }
        }

        private void ValidateProjectStructure(List<GeneratedFile> files, ProjectConfiguration config, ValidationResult result)
        {
            var expectedProjects = new[]
            {
                $"{config.ProjectName}.Framework",
                $"{config.ProjectName}.SourceGenerator",
                $"{config.ProjectName}.Common",
                $"{config.ProjectName}.Server.Data",
                $"{config.ProjectName}.ServiceContracts",
                $"{config.ProjectName}.Server.DataServices",
                $"{config.ProjectName}.Client.Shared",
                $"{config.ProjectName}.Razor",
                $"{config.ProjectName}.WebPortal",
                $"{config.ProjectName}.WebPortal.Client",
                $"{config.ProjectName}.WebAPI",
                $"{config.ProjectName}.ServiceDefaults"
            };

            if (config.UseAspireOrchestration)
            {
                expectedProjects = expectedProjects.Append($"{config.ProjectName}.AppHost").ToArray();
            }

            foreach (var project in expectedProjects)
            {
                var projectFile = files.FirstOrDefault(f => f.RelativePath.Contains($"{project}.csproj"));
                if (projectFile == null)
                {
                    result.Issues.Add($"Missing project file: {project}.csproj");
                }
                else
                {
                    result.ValidatedItems.Add($"✅ Found project: {project}");
                }
            }
        }

        private void ValidateCriticalFiles(List<GeneratedFile> files, ProjectConfiguration config, ValidationResult result)
        {
            var criticalFiles = new[]
            {
                $"{config.ProjectName}.sln",
                $"{config.ProjectName}.Platform/{config.ProjectName}.Razor/Features/Products/ProductListing/ProductListing.razor",
                $"{config.ProjectName}.Platform/{config.ProjectName}.Razor/Features/Products/ProductForm/ProductForm.razor",
                $"{config.ProjectName}.Platform/{config.ProjectName}.Razor/wwwroot/css/products.css",
                $"{config.ProjectName}.Platform/{config.ProjectName}.ServiceContracts/Features/Products/ProductListing/IProductListingDataService.cs",
                $"{config.ProjectName}.Platform/{config.ProjectName}.Server.DataServices/Features/Products/ProductListingServerDataService.cs"
            };

            foreach (var criticalFile in criticalFiles)
            {
                var file = files.FirstOrDefault(f => f.RelativePath.EndsWith(criticalFile) || f.RelativePath.Contains(criticalFile));
                if (file == null)
                {
                    result.Issues.Add($"Missing critical file: {criticalFile}");
                }
                else
                {
                    result.ValidatedItems.Add($"✅ Found critical file: {criticalFile}");
                }
            }
        }

        private void ValidateProjectReferences(List<GeneratedFile> files, ProjectConfiguration config, ValidationResult result)
        {
            // Validate that Razor project references ServiceContracts
            var razorProject = files.FirstOrDefault(f => f.RelativePath.Contains($"{config.ProjectName}.Razor.csproj"));
            if (razorProject != null)
            {
                if (razorProject.Content.Contains($"{config.ProjectName}.ServiceContracts"))
                {
                    result.ValidatedItems.Add("✅ Razor project correctly references ServiceContracts");
                }
                else
                {
                    result.Issues.Add("❌ Razor project missing ServiceContracts reference");
                }
            }

            // Validate that Server.DataServices references ServiceContracts
            var serverDataServicesProject = files.FirstOrDefault(f => f.RelativePath.Contains($"{config.ProjectName}.Server.DataServices.csproj"));
            if (serverDataServicesProject != null)
            {
                if (serverDataServicesProject.Content.Contains($"{config.ProjectName}.ServiceContracts"))
                {
                    result.ValidatedItems.Add("✅ Server.DataServices project correctly references ServiceContracts");
                }
                else
                {
                    result.Issues.Add("❌ Server.DataServices project missing ServiceContracts reference");
                }
            }
        }

        private void ValidateUIComponents(List<GeneratedFile> files, ProjectConfiguration config, ValidationResult result)
        {
            // Validate ProductListing component has improved UI
            var productListing = files.FirstOrDefault(f => f.RelativePath.Contains("ProductListing.razor"));
            if (productListing != null)
            {
                var hasImprovedUI = productListing.Content.Contains("product-card") &&
                                   productListing.Content.Contains("shadow-sm") &&
                                   productListing.Content.Contains("GetStatusBadgeClass") &&
                                   productListing.Content.Contains("Empty State");

                if (hasImprovedUI)
                {
                    result.ValidatedItems.Add("✅ ProductListing has improved UI components");
                }
                else
                {
                    result.Issues.Add("❌ ProductListing missing improved UI components");
                }
            }

            // Validate ProductForm component has improved UI
            var productForm = files.FirstOrDefault(f => f.RelativePath.Contains("ProductForm.razor"));
            if (productForm != null)
            {
                var hasImprovedUI = productForm.Content.Contains("form-floating") &&
                                   productForm.Content.Contains("GetValidationClass") &&
                                   productForm.Content.Contains("alert alert-danger") &&
                                   productForm.Content.Contains("text-danger");

                if (hasImprovedUI)
                {
                    result.ValidatedItems.Add("✅ ProductForm has improved UI components");
                }
                else
                {
                    result.Issues.Add("❌ ProductForm missing improved UI components");
                }
            }

            // Validate CSS file exists and has custom styles
            var cssFile = files.FirstOrDefault(f => f.RelativePath.Contains("products.css"));
            if (cssFile != null)
            {
                var hasCustomStyles = cssFile.Content.Contains("product-card") &&
                                     cssFile.Content.Contains("form-floating") &&
                                     cssFile.Content.Contains("hover") &&
                                     cssFile.Content.Contains("transition");

                if (hasCustomStyles)
                {
                    result.ValidatedItems.Add("✅ Custom CSS file has enhanced styles");
                }
                else
                {
                    result.Issues.Add("❌ Custom CSS file missing enhanced styles");
                }
            }
        }
    }

    public class ValidationResult
    {
        public List<string> Issues { get; set; } = new();
        public List<string> ValidatedItems { get; set; } = new();
        public bool IsValid => Issues.Count == 0;
    }
}
