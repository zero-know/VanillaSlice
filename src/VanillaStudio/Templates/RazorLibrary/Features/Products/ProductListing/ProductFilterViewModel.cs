using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {{ProjectName}}.Common.Enums;
using {{ProjectName}}.Framework;

namespace {{ProjectName}}.Razor.Features.Products.ProductListing
{
    public class ProductFilterViewModel : BaseFilterViewModel
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public ProductStatus? ProductStatus { get; set; }
    }
}
