using {{ProjectName}}.ServiceContracts.Features.Products;

namespace {{ProjectName}}.ClientShared.Features.Products;

public class ProductFormClientDataService : IProductFormDataService
{
    private readonly BaseHttpClient httpClient;

    public ProductFormClientDataService(BaseHttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ProductFormBusinessModel> GetByIdAsync(string id)
    {
        return await httpClient.GetFromJsonAsync<ProductFormBusinessModel>($"api/productForm/{id}");
    }

    public async Task<string> CreateAsync(ProductFormBusinessModel formViewModel)
    {
        return await httpClient.PostAsJsonAsync<string>("api/productForm", formViewModel);
    }

    public async Task<string> UpdateAsync(string id, ProductFormBusinessModel formViewModel)
    {
        return await httpClient.PutAsJsonAsync<string>($"api/productForm/{id}", formViewModel);
    }

    public async Task<int> DeleteAsync(string id)
    {
        return await httpClient.DeleteAsync<int>($"api/productForm/Delete/{id}");
    }
}

