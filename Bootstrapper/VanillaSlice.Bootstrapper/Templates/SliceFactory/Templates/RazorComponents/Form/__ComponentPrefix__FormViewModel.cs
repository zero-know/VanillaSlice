using System.ComponentModel.DataAnnotations;
using {{RootNamespace}}.Framework;

namespace __projectNamespace__;

public class __ComponentPrefix__FormViewModel : ObservableBase
{

    [Required, MaxLength(450)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    // Add other properties as needed
}
