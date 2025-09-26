using {{ProjectName}}.Common.Enums;

namespace {{ProjectName}}.ClientShared.Features.Products;
public class ProductListingViewModel
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public ProductStatus ProductStatus { get; set; }
}
