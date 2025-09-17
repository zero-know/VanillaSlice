using {{RootNamespace}}.Client.Shared;
using {{RootNamespace}}.ServiceContracts.Features.__moduleNamespace__;
using {{RootNamespace}}.Framework.Extensions;

namespace {{ProjectName}}.Client.Shared.Features.__moduleNamespace__;

internal class __ComponentPrefix__SelectListClientDataService : I__ComponentPrefix__SelectListDataService
{
    private readonly BaseHttpClient httpClient;

    public __ComponentPrefix__SelectListClientDataService(BaseHttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<__ComponentPrefix__SelectListBusinessModel>> GetSelectListAsync(__ComponentPrefix__SelectListFilterBusinessModel filter)
    {
        return await httpClient.GetFromJsonAsync<List<__ComponentPrefix__SelectListBusinessModel>>(
            $"api/__ComponentPrefix__SelectList/GetSelectList{filter.ToQueryString()}");
    }
}
