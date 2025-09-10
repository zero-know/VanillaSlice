using ZKnow.VanillaStudio.Models;
using ZKnow.VanillaStudio.Services;
using Microsoft.AspNetCore.Mvc;

namespace ZKnow.VanillaStudio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugGenerationController : ControllerBase
    {
        private readonly EnhancedProjectGenerationService _enhancedService;
        private readonly ProjectGenerationService _basicService;
        private readonly ILogger<DebugGenerationController> _logger;

        public DebugGenerationController(
            EnhancedProjectGenerationService enhancedService,
            ProjectGenerationService basicService,
            ILogger<DebugGenerationController> logger)
        {
            _enhancedService = enhancedService;
            _basicService = basicService;
            _logger = logger;
        }

        [HttpPost("test-enhanced")]
        public async Task<IActionResult> TestEnhancedGeneration()
        {
            try
            {
                var config = new ProjectConfiguration
                {
                    ProjectName = "TestProject",
                    RootNamespace = "TestApp",
                    OutputDirectory = "C:\\temp\\test",
                    PlatformType = PlatformType.WebOnly,
                    ComponentStrategy = ComponentStrategy.Embedded,
                    RenderingMode = RenderingMode.Auto,
                    DatabaseProvider = DatabaseProvider.SqlServer,
                    IncludeDatabase = true,
                    IncludeAuthentication = true,
                    IncludeApiControllers = true,
                    IncludeSampleComponents = true
                };

                _logger.LogInformation("Starting enhanced project generation test...");
                
                var result = await _enhancedService.GenerateProjectAsync(config);

                _logger.LogInformation("Enhanced generation completed. Success: {Success}, Files: {FileCount}", 
                    result.Success, result.GeneratedFiles.Count);

                if (result.Success)
                {
                    var filesByType = result.GeneratedFiles
                        .GroupBy(f => f.Type)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count());

                    var filesByPath = result.GeneratedFiles
                        .Select(f => f.RelativePath)
                        .OrderBy(p => p)
                        .ToList();

                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        totalFiles = result.GeneratedFiles.Count,
                        filesByType = filesByType,
                        sampleFiles = filesByPath.Take(20).ToList(),
                        allFiles = filesByPath,
                        downloadUrl = result.DownloadUrl,
                        errors = result.Errors
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        errors = result.Errors,
                        fileCount = result.GeneratedFiles.Count
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in enhanced generation test");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Test failed with exception",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("test-basic")]
        public async Task<IActionResult> TestBasicGeneration()
        {
            try
            {
                var config = new ProjectConfiguration
                {
                    ProjectName = "TestProject",
                    RootNamespace = "TestApp",
                    OutputDirectory = "C:\\temp\\test",
                    PlatformType = PlatformType.WebOnly,
                    ComponentStrategy = ComponentStrategy.Embedded,
                    RenderingMode = RenderingMode.Auto,
                    DatabaseProvider = DatabaseProvider.SqlServer,
                    IncludeDatabase = true,
                    IncludeAuthentication = true,
                    IncludeApiControllers = true,
                    IncludeSampleComponents = true
                };

                _logger.LogInformation("Starting basic project generation test...");
                
                var result = await _basicService.GenerateProjectAsync(config);

                _logger.LogInformation("Basic generation completed. Success: {Success}, Files: {FileCount}", 
                    result.Success, result.GeneratedFiles.Count);

                if (result.Success)
                {
                    var filesByType = result.GeneratedFiles
                        .GroupBy(f => f.Type)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count());

                    var filesByPath = result.GeneratedFiles
                        .Select(f => f.RelativePath)
                        .OrderBy(p => p)
                        .ToList();

                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        totalFiles = result.GeneratedFiles.Count,
                        filesByType = filesByType,
                        sampleFiles = filesByPath.Take(20).ToList(),
                        allFiles = filesByPath,
                        downloadUrl = result.DownloadUrl,
                        errors = result.Errors
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        errors = result.Errors,
                        fileCount = result.GeneratedFiles.Count
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in basic generation test");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Test failed with exception",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("compare")]
        public async Task<IActionResult> CompareGenerations()
        {
            try
            {
                var config = new ProjectConfiguration
                {
                    ProjectName = "CompareTest",
                    RootNamespace = "CompareApp",
                    OutputDirectory = "C:\\temp\\compare",
                    PlatformType = PlatformType.WebOnly,
                    ComponentStrategy = ComponentStrategy.Embedded,
                    RenderingMode = RenderingMode.Auto,
                    DatabaseProvider = DatabaseProvider.SqlServer,
                    IncludeDatabase = true,
                    IncludeAuthentication = false,
                    IncludeApiControllers = true,
                    IncludeSampleComponents = true
                };

                var basicResult = await _basicService.GenerateProjectAsync(config);
                var enhancedResult = await _enhancedService.GenerateProjectAsync(config);

                return Ok(new
                {
                    basic = new
                    {
                        success = basicResult.Success,
                        fileCount = basicResult.GeneratedFiles.Count,
                        files = basicResult.GeneratedFiles.Select(f => f.RelativePath).OrderBy(p => p).ToList()
                    },
                    enhanced = new
                    {
                        success = enhancedResult.Success,
                        fileCount = enhancedResult.GeneratedFiles.Count,
                        files = enhancedResult.GeneratedFiles.Select(f => f.RelativePath).OrderBy(p => p).ToList()
                    },
                    differences = new
                    {
                        onlyInBasic = basicResult.GeneratedFiles.Select(f => f.RelativePath)
                            .Except(enhancedResult.GeneratedFiles.Select(f => f.RelativePath)).ToList(),
                        onlyInEnhanced = enhancedResult.GeneratedFiles.Select(f => f.RelativePath)
                            .Except(basicResult.GeneratedFiles.Select(f => f.RelativePath)).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in generation comparison");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}
