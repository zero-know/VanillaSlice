using System.ComponentModel.DataAnnotations;
using {{RootNamespace}}.Framework;

namespace __projectNamespace__;

public class __ComponentPrefix__FormBusinessModel
{

    [Required, MaxLength(450)]
    public string Name { get; set; } = string.Empty;


    // Add other properties as needed
}
