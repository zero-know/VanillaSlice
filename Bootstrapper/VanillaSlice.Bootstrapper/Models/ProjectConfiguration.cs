using System.ComponentModel.DataAnnotations;

namespace VanillaSlice.Bootstrapper.Models
{
    public class ProjectConfiguration
    {
        [Required]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Root Namespace")]
        public string RootNamespace { get; set; } = string.Empty;

        [Display(Name = "Output Directory")]
        public string OutputDirectory { get; set; } = string.Empty;

        [Display(Name = "Project Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Author Name")]
        public string AuthorName { get; set; } = string.Empty;

        // Platform Selection
        public PlatformType PlatformType { get; set; } = PlatformType.WebOnly;

        // Razor Component Strategy
        public ComponentStrategy ComponentStrategy { get; set; } = ComponentStrategy.CommonLibrary;

        // Rendering Mode
        public RenderingMode RenderingMode { get; set; } = RenderingMode.Auto;

        // Additional Features
        public bool IncludeAuthentication { get; set; } = true;
        public bool IncludeDatabase { get; set; } = true;
        public bool IncludeApiControllers { get; set; } = true;
        public bool IncludeSampleComponents { get; set; } = true;
        public bool IncludeSampleData { get; set; } = true;

        // Database Configuration
        public DatabaseProvider DatabaseProvider { get; set; } = DatabaseProvider.SqlServer;
        public string ConnectionStringName { get; set; } = "DefaultConnection";

        // Advanced Options
        public bool UseAspireOrchestration { get; set; } = false;
        public bool IncludeDockerSupport { get; set; } = false;
        public bool IncludeTestProjects { get; set; } = false;

        public ProjectConfiguration()
        {
#if DEBUG
            ProjectName = "ZKnowledge.Enterprise";
            RootNamespace = "ZKnowledge.Enterprise";
            OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ZKnowledge.Enterprise");
#else
            // Set default output directory to current directory + project name
            if (!string.IsNullOrEmpty(ProjectName))
            {
                OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), ProjectName);
            }
#endif
        }
    }

    public enum PlatformType
    {
        [Display(Name = "Web Application Only")]
        WebOnly = 1,

        [Display(Name = "Web Application + MAUI Mobile App")]
        WebAndMaui = 2
    }

    public enum ComponentStrategy
    {
        [Display(Name = "Common Razor Library (Reusable across projects)")]
        CommonLibrary = 1,

        [Display(Name = "Embedded Components (Directly in WebPortal project)")]
        Embedded = 2
    }

    public enum RenderingMode
    {
        [Display(Name = "Auto Render (Server + WebAssembly hybrid)")]
        Auto = 1,

        [Display(Name = "Server-Side Interactive Only")]
        ServerOnly = 2,

        [Display(Name = "Static Server-Side Rendering (SSR) Only")]
        StaticSSR = 3
    }

    public enum DatabaseProvider
    {
        [Display(Name = "SQL Server")]
        SqlServer = 1,

        [Display(Name = "SQLite")]
        SQLite = 2,

        [Display(Name = "PostgreSQL")]
        PostgreSQL = 3,

        [Display(Name = "No Database")]
        None = 4
    }

    public class ProjectGenerationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public string? DownloadUrl { get; set; }
        public string? ProjectPath { get; set; }
        public List<GeneratedFile> GeneratedFiles { get; set; } = new();
        public byte[]? ZipData { get; set; }
        public string? ZipFileName { get; set; }
    }

    public class GeneratedFile
    {
        public string RelativePath { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public FileType Type { get; set; }
    }

    public enum FileType
    {
        CSharpCode,
        ProjectFile,
        SolutionFile,
        RazorComponent,
        JsonConfig,
        Other
    }
}
