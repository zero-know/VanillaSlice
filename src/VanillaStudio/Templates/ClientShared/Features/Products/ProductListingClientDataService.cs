using {{ProjectName}}.Framework;
using {{ProjectName}}.Framework.Extensions;
using {{ProjectName}}.ServiceContracts.Features.Products;

namespace {{ProjectName}}.Client.Shared.Features.Products
{
    internal class ProductListingClientDataService : IProductListingDataService
    {
        private readonly BaseHttpClient httpClient;

        public ProductListingClientDataService(BaseHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PagedDataList<ProductListingBusinessModel>> GetPaginatedItemsAsync(ProductFilterBusinessModel filterViewModel)
        {
            return await httpClient.GetFromJsonAsync<PagedDataList<ProductListingBusinessModel>>("api/productListing/GetPaginatedItems" + filterViewModel.ToQueryString());
        }
    }
}
