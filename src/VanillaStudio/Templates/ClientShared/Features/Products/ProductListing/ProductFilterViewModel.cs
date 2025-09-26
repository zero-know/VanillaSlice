using {{ProjectName}}.Common.Enums;
using {{ProjectName}}.Framework;

namespace {{ProjectName}}.ClientShared.Features.Products;

public class ProductFilterViewModel : BaseFilterViewModel
{
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public ProductStatus? ProductStatus { get; set; }
}
