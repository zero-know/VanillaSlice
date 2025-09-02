using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using {{RootNamespace}}.SliceFactory.Components.Pages;
using {{RootNamespace}}.SliceFactory.Services;

namespace {{RootNamespace}}.SliceFactory.Models;
public class Feature
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ComponentPrefix { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ModuleNamespace { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ProjectNamespace { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string PrimaryKeyType { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string BasePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DirectoryName { get; set; } = string.Empty;

    public bool HasForm { get; set; }
    public bool HasListing { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// JSON serialized configuration from webportal-profile.json
    /// </summary>
    public string? ProfileConfiguration { get; set; }

    /// <summary>
    /// Navigation property for generated files
    /// </summary>
    public virtual ICollection<FeatureFile> Files { get; set; } = new List<FeatureFile>();

    /// <summary>
    /// Navigation property for feature projects
    /// </summary>
    public virtual ICollection<FeatureProject> Projects { get; set; } = new List<FeatureProject>();
}

/// <summary>
/// Represents a file generated for a feature
/// </summary>
public class FeatureFile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FeatureId { get; set; }

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ProjectType { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SliceType { get; set; } = string.Empty; // Form or Listing

    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Whether the file still exists on disk
    /// </summary>
    public bool Exists { get; set; } = true;

    /// <summary>
    /// Navigation property back to feature
    /// </summary>
    [ForeignKey(nameof(FeatureId))]
    public virtual Feature Feature { get; set; } = null!;
}

/// <summary>
/// Represents the relationship between a feature and project types
/// </summary>
public class FeatureProject
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FeatureId { get; set; }

    [Required]
    public ProjectType ProjectType { get; set; }

    [Required]
    [MaxLength(500)]
    public string ProjectPath { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ProjectNamespace { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property back to feature
    /// </summary>
    [ForeignKey(nameof(FeatureId))]
    public virtual Feature Feature { get; set; } = null!;
}

/// <summary>
/// Tree node for hierarchical display
/// </summary>
public class FeatureTreeNode
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Module, Feature, ProjectType, File
    public bool IsExpanded { get; set; }
    public bool HasChildren => Children.Any();
    public List<FeatureTreeNode> Children { get; set; } = new();

    // Optional data for different node types
    public Feature? Feature { get; set; }
    public FeatureFile? File { get; set; }
    public ProjectType? ProjectType { get; set; }
    public string? ModuleNamespace { get; set; }
}