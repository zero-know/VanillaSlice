using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {{ProjectName}}.Common.Enums;
using {{ProjectName}}.Framework;

namespace {{ProjectName}}.Razor.Features.Products
{
    public class ProductFormViewModel : ObservableBase
{
    [MaxLength(50)]
    public string? Id { get; set; }

    [MaxLength(450), Required]
    public string Name { get; set; } = null!;

    [MaxLength(4000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Enter a valid product price")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Select a Product Status")]
    public ProductStatus? ProductStatus { get; set; }
}
}
