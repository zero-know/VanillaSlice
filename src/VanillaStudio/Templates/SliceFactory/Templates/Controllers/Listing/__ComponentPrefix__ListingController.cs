using Microsoft.AspNetCore.Mvc;
using {{RootNamespace}}.Framework;
using {{RootNamespace}}.ServiceContracts.Features.__ComponentPrefix__;

namespace {{RootNamespace}}.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class __ComponentPrefix__ListingController : ControllerBase, I__ComponentPrefix__ListingDataService
{
    private readonly I__ComponentPrefix__ListingDataService _service;

    public __ComponentPrefix__ListingController(I__ComponentPrefix__ListingDataService service)
    {
        _service = service;
    }

    [HttpGet("GetPaginatedItems")]
    public async Task<PagedDataList<__ComponentPrefix__ListingBusinessModel>> GetPaginatedItemsAsync([FromQuery] __ComponentPrefix__FilterBusinessModel filter)
    {
        return await _service.GetPaginatedItemsAsync(filter);
    }
}


