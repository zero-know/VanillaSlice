using {{ProjectName}}.Framework;
using {{ProjectName}}.ServiceContracts.Features.Products;

namespace {{ProjectName}}.ClientShared.Features.Products;

public class ProductListingOfflineDataService : IProductListingDataService
{
    public Task<PagedDataList<ProductListingBusinessModel>> GetPaginatedItemsAsync(ProductFilterBusinessModel filterViewModel)
    {
        throw new NotImplementedException();
    }
}