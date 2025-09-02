using {{RootNamespace}}.SliceFactory.Services;

namespace {{RootNamespace}}.SliceFactory.Models;

public class PlacementGuidance
{
    public List<FeatureFilePreview> NewFiles { get; set; } = new();
    public List<ExistingFileInfo> ExistingFiles { get; set; } = new();
    public List<ConflictWarning> Conflicts { get; set; } = new();
    public NamespaceHierarchy NamespaceStructure { get; set; } = new();
    public List<PlacementSuggestion> Suggestions { get; set; } = new();
    public bool HasConflicts => Conflicts.Any();
    public bool HasWarnings => Conflicts.Any(c => c.Severity == ConflictSeverity.Warning);
    public bool HasErrors => Conflicts.Any(c => c.Severity == ConflictSeverity.Error);
}

public class ExistingFileInfo
{
    public string FilePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public string ProjectType { get; set; } = "";
    public string SliceType { get; set; } = "";
    public DateTime LastModified { get; set; }
    public long FileSize { get; set; }
    public bool WillBeOverwritten { get; set; }
}

public class ConflictWarning
{
    public ConflictType Type { get; set; }
    public ConflictSeverity Severity { get; set; }
    public string Message { get; set; } = "";
    public string Details { get; set; } = "";
    public string FilePath { get; set; } = "";
    public List<string> Suggestions { get; set; } = new();
}

public class NamespaceHierarchy
{
    public string RootNamespace { get; set; } = "";
    public List<NamespaceNode> Nodes { get; set; } = new();
}

public class NamespaceNode
{
    public string Name { get; set; } = "";
    public string FullPath { get; set; } = "";
    public NodeType Type { get; set; }
    public List<NamespaceNode> Children { get; set; } = new();
    public bool IsNew { get; set; }
    public bool HasConflict { get; set; }
    public int ExistingFeatureCount { get; set; }
}

public class PlacementSuggestion
{
    public SuggestionType Type { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string RecommendedAction { get; set; } = "";
    public Dictionary<string, string> Parameters { get; set; } = new();
}

public enum ConflictType
{
    DuplicateFeatureName,
    FileOverwrite,
    NamespaceConflict,
    NamingConvention,
    DirectoryStructure
}

public enum ConflictSeverity
{
    Info,
    Warning,
    Error
}

public enum NodeType
{
    Module,
    Feature,
    Project,
    File,
    Directory
}

public enum SuggestionType
{
    AlternativeName,
    AlternativeNamespace,
    AlternativeLocation,
    NamingConvention,
    BestPractice
}
