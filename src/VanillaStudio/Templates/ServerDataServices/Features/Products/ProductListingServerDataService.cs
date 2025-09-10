using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {{ProjectName}}.Framework;
using {{ProjectName}}.Server.Data;
using {{ProjectName}}.ServiceContracts.Features.Products;

namespace {{ProjectName}}.Server.DataServices.Features.Products
{
    internal class ProductListingServerDataService : ListingServerDataService<ProductListingBusinessModel, ProductFilterBusinessModel>, IProductListingDataService
    {
        private readonly AppDbContext context;

        public ProductListingServerDataService(AppDbContext context)
        {
            this.context = context;
        }

        public override IQueryable<ProductListingBusinessModel> GetQuery(ProductFilterBusinessModel filterBusinessObject)
        {
            return (from p in context.Products
                    where (string.IsNullOrEmpty(filterBusinessObject.Name) || p.Name.Contains(filterBusinessObject.Name)) &&
                          (!filterBusinessObject.MinPrice.HasValue || p.Price >= filterBusinessObject.MinPrice.Value) &&
                          (!filterBusinessObject.MaxPrice.HasValue || p.Price <= filterBusinessObject.MaxPrice.Value) &&
                          (!filterBusinessObject.ProductStatus.HasValue || p.ProductStatus == filterBusinessObject.ProductStatus.Value)
                    select new ProductListingBusinessModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CreatedAt = p.CreatedAt,
                        ProductStatus = p.ProductStatus
                    });

        }
    }
}
