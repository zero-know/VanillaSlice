using {{ProjectName}}.ServiceContracts.Features.Products;

namespace {{ProjectName}}.ClientShared.Features.Products;

public class ProductFormOfflineDataService : IProductFormDataService
{
    public Task<string> CreateAsync(ProductFormBusinessModel formViewModel)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<ProductFormBusinessModel> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateAsync(string id, ProductFormBusinessModel formViewModel)
    {
        throw new NotImplementedException();
    }
}