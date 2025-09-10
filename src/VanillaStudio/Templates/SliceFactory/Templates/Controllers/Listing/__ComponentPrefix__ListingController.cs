using Microsoft.AspNetCore.Mvc;
using {{RootNamespace}}.ServiceContracts.Features.__moduleNamespace__;

namespace __projectNamespace__;

[ApiController]
[Route("api/[controller]")]
public class __ComponentPrefix__ListingController : ControllerBase
{
    private readonly I__ComponentPrefix__ListingDataService _service;

    public __ComponentPrefix__ListingController(I__ComponentPrefix__ListingDataService service)
    {
        _service = service;
    }

    [HttpGet("GetPaginatedItems")]
    public async Task<IActionResult> GetPaginatedItemsAsync([FromQuery] __ComponentPrefix__FilterBusinessModel filter)
    {
        var result = await _service.GetPaginatedItemsAsync(filter);
        return Ok(result);
    }
}


