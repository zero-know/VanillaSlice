namespace {{RootNamespace}}.SliceFactory.Services;

/// <summary>
/// Service for automatically detecting solution and project paths
/// </summary>
public class PathDetectionService
{
    private readonly ILogger<PathDetectionService> _logger;
    private readonly IWebHostEnvironment _environment;

    public PathDetectionService(ILogger<PathDetectionService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Automatically detects the solution root directory
    /// </summary>
    public string DetectSolutionRoot()
    {
        try
        {
            // Start from the current application directory (SliceFactory location)
            var currentDirectory = _environment.ContentRootPath;
            var directory = new DirectoryInfo(currentDirectory);

            _logger.LogInformation("Starting path detection from: {CurrentDirectory}", currentDirectory);

            // Look for solution file in current directory and parent directories
            while (directory != null)
            {
                // Check if this directory contains a .sln file
                var solutionFiles = directory.GetFiles("*.sln");
                if (solutionFiles.Length > 0)
                {
                    _logger.LogInformation("Found solution root at: {SolutionRoot}", directory.FullName);
                    return directory.FullName;
                }

                // Check if this directory contains typical project structure indicators
                if (HasProjectStructureIndicators(directory))
                {
                    _logger.LogInformation("Found project structure indicators at: {ProjectRoot}", directory.FullName);
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            // Fallback: use parent directory of SliceFactory
            var fallbackPath = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;
            _logger.LogWarning("Could not detect solution root, using fallback: {FallbackPath}", fallbackPath);
            return fallbackPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting solution root");
            return _environment.ContentRootPath;
        }
    }

    /// <summary>
    /// Detects project paths relative to the solution root
    /// </summary>
    public Dictionary<string, string> DetectProjectPaths(string solutionRoot)
    {
        var projectPaths = new Dictionary<string, string>();

        try
        {
            // Look for common project directories
            var commonProjectPatterns = new[]
            {
                "*Platform*/*ServiceContracts*",
                "*Platform*/*Server.DataServices*", 
                "*Platform*/*Client.Shared*",
                "*Platform*/*Razor*",
                "*WebPortal*/*WebPortal.Client*",
                "*WebPortal*/*WebPortal*",
                "*HybridApp*"
            };

            foreach (var pattern in commonProjectPatterns)
            {
                var matchingDirs = Directory.GetDirectories(solutionRoot, pattern, SearchOption.AllDirectories);
                foreach (var dir in matchingDirs)
                {
                    var relativePath = Path.GetRelativePath(solutionRoot, dir);
                    var projectName = Path.GetFileName(dir);
                    
                    if (!projectPaths.ContainsKey(projectName))
                    {
                        projectPaths[projectName] = relativePath;
                        _logger.LogDebug("Detected project: {ProjectName} at {RelativePath}", projectName, relativePath);
                    }
                }
            }

            _logger.LogInformation("Detected {ProjectCount} project paths", projectPaths.Count);
            return projectPaths;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting project paths");
            return projectPaths;
        }
    }

    /// <summary>
    /// Gets the base path for feature generation (solution root)
    /// </summary>
    public string GetFeatureGenerationBasePath()
    {
        return DetectSolutionRoot();
    }

    /// <summary>
    /// Validates that the detected paths are correct
    /// </summary>
    public bool ValidateDetectedPaths(string solutionRoot, Dictionary<string, string> projectPaths)
    {
        try
        {
            // Check if solution root exists
            if (!Directory.Exists(solutionRoot))
            {
                _logger.LogWarning("Solution root does not exist: {SolutionRoot}", solutionRoot);
                return false;
            }

            // Check if at least some expected projects exist
            var expectedProjects = new[] { "ServiceContracts", "Server.DataServices", "Client.Shared" };
            var foundProjects = projectPaths.Keys.Where(k => expectedProjects.Any(ep => k.Contains(ep))).Count();

            if (foundProjects == 0)
            {
                _logger.LogWarning("No expected projects found in detected paths");
                return false;
            }

            _logger.LogInformation("Path validation successful. Found {FoundProjects} expected projects", foundProjects);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating detected paths");
            return false;
        }
    }

    private bool HasProjectStructureIndicators(DirectoryInfo directory)
    {
        try
        {
            // Look for typical project structure indicators
            var indicators = new[]
            {
                "Platform",
                "WebPortal", 
                "HybridApp",
                "ServiceContracts",
                "Server.DataServices"
            };

            var subdirectories = directory.GetDirectories();
            var foundIndicators = subdirectories.Count(d => indicators.Any(i => d.Name.Contains(i)));

            return foundIndicators >= 2; // At least 2 indicators suggest this is the project root
        }
        catch
        {
            return false;
        }
    }
}
