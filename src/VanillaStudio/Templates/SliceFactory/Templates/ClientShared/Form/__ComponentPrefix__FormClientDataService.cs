using {{RootNamespace}}.Client.Shared;
using {{RootNamespace}}.Framework;
using {{RootNamespace}}.ServiceContracts.Features.__moduleNamespace__;

namespace {{ProjectName}}.Client.Shared.Features.__moduleNamespace__;

internal class __ComponentPrefix__FormClientDataService : I__ComponentPrefix__FormDataService
{
    private readonly BaseHttpClient httpClient;

    public __ComponentPrefix__FormClientDataService(BaseHttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<__ComponentPrefix__FormBusinessModel> GetByIdAsync(__primaryKeyType__ id)
    {
        return await httpClient.GetFromJsonAsync<__ComponentPrefix__FormBusinessModel>($"api/__ComponentPrefix__Form/{id}");
    }

    public async Task<__primaryKeyType__> CreateAsync(__ComponentPrefix__FormBusinessModel model)
    {
        return await httpClient.PostAsJsonAsync<__primaryKeyType__>("api/__ComponentPrefix__Form", model);
    }

    public async Task<__primaryKeyType__> UpdateAsync(__primaryKeyType__ id, __ComponentPrefix__FormBusinessModel model)
    {
        return await httpClient.PutAsJsonAsync<__primaryKeyType__>($"api/__ComponentPrefix__Form/{id}", model); 
    }

    public async Task<int> DeleteAsync(__primaryKeyType__ id)
    {
        return await httpClient.DeleteAsync<int>($"api/__ComponentPrefix__Form/{id}");
    }
}
