using VanillaSlice.Bootstrapper.Models;
using VanillaSlice.Bootstrapper.Services;
using Microsoft.AspNetCore.Mvc;

namespace VanillaSlice.Bootstrapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnhancedProjectGenerationController : ControllerBase
    {
        private readonly EnhancedProjectGenerationService _projectGenerationService;
        private readonly ILogger<EnhancedProjectGenerationController> _logger;

        public EnhancedProjectGenerationController(
            EnhancedProjectGenerationService projectGenerationService,
            ILogger<EnhancedProjectGenerationController> logger)
        {
            _projectGenerationService = projectGenerationService;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateProject([FromBody] ProjectConfiguration config)
        {
            try
            {
                _logger.LogInformation("Received enhanced project generation request for {ProjectName}", config.ProjectName);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _projectGenerationService.GenerateProjectAsync(config);

                if (result.Success)
                {
                    _logger.LogInformation("Enhanced project generation completed successfully for {ProjectName}", config.ProjectName);
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        filesGenerated = result.GeneratedFiles.Count,
                        zipData = result.ZipData != null ? Convert.ToBase64String(result.ZipData) : null,
                        zipFileName = result.ZipFileName,
                        generatedFiles = result.GeneratedFiles.Select(f => new {
                            path = f.RelativePath,
                            type = f.Type.ToString()
                        }).ToList()
                    });
                }
                else
                {
                    _logger.LogWarning("Enhanced project generation failed for {ProjectName}: {Errors}", 
                        config.ProjectName, string.Join(", ", result.Errors));
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
                _logger.LogError(ex, "Error in enhanced project generation for {ProjectName}", config.ProjectName);
                return StatusCode(500, new
                {
                    success = false,
                    message = "An internal server error occurred during project generation",
                    error = ex.Message
                });
            }
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadProject(string fileName)
        {
            try
            {
                var downloadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "downloads");
                var filePath = Path.Combine(downloadsPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning("Download file not found: {FilePath}", filePath);
                    return NotFound(new { message = "File not found" });
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var contentType = "application/zip";

                _logger.LogInformation("Serving download file: {FileName}", fileName);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file {FileName}", fileName);
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while downloading the file",
                    error = ex.Message
                });
            }
        }

        [HttpGet("validate")]
        public IActionResult ValidateConfiguration([FromQuery] string projectName, [FromQuery] string rootNamespace)
        {
            try
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(projectName))
                    errors.Add("Project name is required");

                if (string.IsNullOrWhiteSpace(rootNamespace))
                    errors.Add("Root namespace is required");

                // Validate project name format
                if (!string.IsNullOrWhiteSpace(projectName) && 
                    !System.Text.RegularExpressions.Regex.IsMatch(projectName, @"^[a-zA-Z][a-zA-Z0-9._]*$"))
                {
                    errors.Add("Project name must start with a letter and contain only letters, numbers, dots, and underscores");
                }

                // Validate namespace format
                if (!string.IsNullOrWhiteSpace(rootNamespace) && 
                    !System.Text.RegularExpressions.Regex.IsMatch(rootNamespace, @"^[a-zA-Z][a-zA-Z0-9._]*$"))
                {
                    errors.Add("Root namespace must start with a letter and contain only letters, numbers, dots, and underscores");
                }

                return Ok(new
                {
                    isValid = errors.Count == 0,
                    errors = errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating configuration");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while validating the configuration",
                    error = ex.Message
                });
            }
        }

        [HttpGet("templates")]
        public IActionResult GetAvailableTemplates()
        {
            try
            {
                var templates = new[]
                {
                    new { 
                        name = "Complete Blazor Solution", 
                        description = "Full Blazor solution with Framework.Core, SourceGenerator, Platform projects, and sample CRUD",
                        features = new[] { "Framework.Core with ListingBase/FormBase", "SourceGenerator project", "BaseHttpClient implementations", "Sample Product CRUD", "Database support", "Authentication ready" }
                    },
                    new { 
                        name = "Web-Only Solution", 
                        description = "Blazor web application without MAUI support",
                        features = new[] { "Server-side rendering", "Interactive components", "Database integration" }
                    },
                    new { 
                        name = "Web + MAUI Solution", 
                        description = "Blazor web application with MAUI mobile app support",
                        features = new[] { "Shared components", "Cross-platform support", "Unified codebase" }
                    }
                };

                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available templates");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while getting available templates",
                    error = ex.Message
                });
            }
        }

        [HttpGet("status")]
        public IActionResult GetServiceStatus()
        {
            try
            {
                var status = new
                {
                    service = "Enhanced Project Generation Service",
                    version = "2.0.0",
                    status = "Running",
                    features = new[]
                    {
                        "Complete Framework.Core generation",
                        "SourceGenerator project included",
                        "BaseHttpClient implementations",
                        "Sample CRUD components",
                        "Multiple database providers",
                        "Authentication support",
                        "Platform projects generation"
                    },
                    timestamp = DateTime.UtcNow
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service status");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while getting service status",
                    error = ex.Message
                });
            }
        }
    }
}
