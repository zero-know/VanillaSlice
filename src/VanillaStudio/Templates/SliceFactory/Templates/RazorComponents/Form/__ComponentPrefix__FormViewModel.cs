using System.ComponentModel.DataAnnotations;
using {{RootNamespace}}.Framework;

namespace {{RootNamespace}}.ClientShared.Features.__ComponentPrefix__;

public class __ComponentPrefix__FormViewModel : ObservableBase
{
    [MaxLength(50)]
    public __primaryKeyType__? Id { get; set; }

    [Required, MaxLength(450)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    // Add other properties as needed based on your entity
}
