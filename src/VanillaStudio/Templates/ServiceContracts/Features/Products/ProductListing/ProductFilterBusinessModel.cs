using {{ProjectName}}.Common.Enums;
using {{ProjectName}}.Framework;

namespace {{ProjectName}}.ServiceContracts.Features.Products
{
    public class ProductFilterBusinessModel : BaseFilterBusinessObject
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public ProductStatus? ProductStatus { get; set; }
    }
}
