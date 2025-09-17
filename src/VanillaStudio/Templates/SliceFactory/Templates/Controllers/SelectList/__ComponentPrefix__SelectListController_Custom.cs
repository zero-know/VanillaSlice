using Microsoft.AspNetCore.Mvc;
using {{RootNamespace}}.ServiceContracts.Features.__moduleNamespace__;

namespace __projectNamespace__;

[ApiController]
[Route("api/[controller]")]
public class __ComponentPrefix__SelectListController : ControllerBase, I__ComponentPrefix__SelectListDataService
{
    private readonly I__ComponentPrefix__SelectListDataService _service;

    public __ComponentPrefix__SelectListController(I__ComponentPrefix__SelectListDataService service)
    {
        _service = service;
    }

    [HttpGet("GetSelectList")]
    public async Task<List<__ComponentPrefix__SelectListBusinessModel>> GetSelectListAsync([FromQuery] __ComponentPrefix__SelectListFilterBusinessModel filter)
    {
        return await _service.GetSelectListAsync(filter);
    }
}