namespace {{RootNamespace}}.SliceFactory.Services;

/// <summary>
/// Provides the content root path for template resolution
/// </summary>
public interface IContentRootProvider
{
    /// <summary>
    /// Gets the content root path where templates are located
    /// </summary>
    string ContentRootPath { get; }
}

/// <summary>
/// Web-based implementation that uses IWebHostEnvironment
/// </summary>
public class WebContentRootProvider : IContentRootProvider
{
    private readonly IWebHostEnvironment _environment;

    public WebContentRootProvider(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string ContentRootPath => _environment.ContentRootPath;
}

/// <summary>
/// CLI-based implementation that uses a specified path
/// </summary>
public class CliContentRootProvider : IContentRootProvider
{
    public CliContentRootProvider(string? contentRootPath = null)
    {
        // Try to find the SliceFactory project directory
        ContentRootPath = contentRootPath ?? FindSliceFactoryPath() ?? AppContext.BaseDirectory;
    }

    public string ContentRootPath { get; }

    private static string? FindSliceFactoryPath()
    {
        // Start from current directory and look for Templates folder
        var current = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(current);

        while (directory != null)
        {
            // Check if this directory contains Templates folder (SliceFactory root)
            var templatesPath = Path.Combine(directory.FullName, "Templates");
            if (Directory.Exists(templatesPath))
            {
                return directory.FullName;
            }

            // Check if we're in bin folder, look for source
            if (directory.Name == "Debug" || directory.Name == "Release")
            {
                // Go up to find the project root
                var projectRoot = directory.Parent?.Parent;
                if (projectRoot != null)
                {
                    templatesPath = Path.Combine(projectRoot.FullName, "Templates");
                    if (Directory.Exists(templatesPath))
                    {
                        return projectRoot.FullName;
                    }
                }
            }

            // Look for SliceFactory project folder
            var sliceFactoryPath = Directory.GetDirectories(directory.FullName, "*SliceFactory*", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
            if (sliceFactoryPath != null)
            {
                templatesPath = Path.Combine(sliceFactoryPath, "Templates");
                if (Directory.Exists(templatesPath))
                {
                    return sliceFactoryPath;
                }
            }

            directory = directory.Parent;
        }

        return null;
    }
}
