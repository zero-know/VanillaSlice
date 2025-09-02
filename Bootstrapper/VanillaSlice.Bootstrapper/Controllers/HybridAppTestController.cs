using VanillaSlice.Bootstrapper.Models;
using VanillaSlice.Bootstrapper.Services;
using Microsoft.AspNetCore.Mvc;

namespace VanillaSlice.Bootstrapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HybridAppTestController : ControllerBase
    {
        private readonly EnhancedProjectGenerationService _projectGenerationService;
        private readonly ILogger<HybridAppTestController> _logger;

        public HybridAppTestController(
            EnhancedProjectGenerationService projectGenerationService,
            ILogger<HybridAppTestController> logger)
        {
            _projectGenerationService = projectGenerationService;
            _logger = logger;
        }

        [HttpPost("test-hybrid-generation")]
        public async Task<IActionResult> TestHybridGeneration()
        {
            try
            {
                var config = new ProjectConfiguration
                {
                    ProjectName = "TestHybridApp",
                    RootNamespace = "TestHybridApp",
                    OutputDirectory = Path.Combine(Path.GetTempPath(), "TestHybridApp"),
                    PlatformType = PlatformType.WebAndMaui,
                    ComponentStrategy = ComponentStrategy.CommonLibrary,
                    RenderingMode = RenderingMode.Auto,
                    DatabaseProvider = DatabaseProvider.SqlServer,
                    IncludeDatabase = true,
                    IncludeAuthentication = true,
                    IncludeApiControllers = true,
                    IncludeSampleComponents = true
                };

                _logger.LogInformation("Testing HybridApp generation with config: {@Config}", config);

                var result = await _projectGenerationService.GenerateProjectAsync(config);

                if (result.Success)
                {
                    var hybridAppFiles = result.GeneratedFiles
                        .Where(f => f.RelativePath.Contains("HybridApp"))
                        .ToList();

                    return Ok(new
                    {
                        success = true,
                        message = "HybridApp generation test completed successfully",
                        totalFiles = result.GeneratedFiles.Count,
                        hybridAppFiles = hybridAppFiles.Count,
                        hybridAppFilesList = hybridAppFiles.Select(f => f.RelativePath).ToList(),
                        downloadUrl = result.DownloadUrl
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        errors = result.Errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing HybridApp generation");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred during HybridApp generation test",
                    error = ex.Message
                });
            }
        }
    }
}
