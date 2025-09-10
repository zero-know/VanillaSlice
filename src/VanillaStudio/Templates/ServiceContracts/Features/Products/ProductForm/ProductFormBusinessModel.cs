using System.ComponentModel.DataAnnotations;
using {{ProjectName}}.Common.Enums;

namespace {{ProjectName}}.ServiceContracts.Features.Products
{
    public class ProductFormBusinessModel
    {
        public string? Id { get; set; }

        [Required, MaxLength(450)]
        public string Name { get; set; } = null!;

        [MaxLength(4000)]
        public string? Description { get; set; }

        [Required]
        public decimal? Price { get; set; }

        public ProductStatus ProductStatus { get; set; }
    }
}
