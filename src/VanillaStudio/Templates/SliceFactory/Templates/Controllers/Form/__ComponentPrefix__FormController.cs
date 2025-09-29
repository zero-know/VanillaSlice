using Microsoft.AspNetCore.Mvc;
using {{RootNamespace}}.ServiceContracts.Features.__ComponentPrefix__;

namespace {{RootNamespace}}.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class __ComponentPrefix__FormController : ControllerBase, I__ComponentPrefix__FormDataService
{
    private readonly I__ComponentPrefix__FormDataService _service;

    public __ComponentPrefix__FormController(I__ComponentPrefix__FormDataService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<__ComponentPrefix__FormBusinessModel> GetByIdAsync(__primaryKeyType__ id)
    {
        return await _service.GetByIdAsync(id);
    }

    [HttpPost]
    public async Task<__primaryKeyType__> CreateAsync([FromBody] __ComponentPrefix__FormBusinessModel model)
    {
        return await _service.CreateAsync(model);
    }

    [HttpPut("{id}")]
    public async Task<__primaryKeyType__> UpdateAsync(__primaryKeyType__ id, [FromBody] __ComponentPrefix__FormBusinessModel model)
    {
        return await _service.UpdateAsync(id, model);
    }

    [HttpDelete("{id}")]
    public async Task<int> DeleteAsync(__primaryKeyType__ id)
    {
        return await _service.DeleteAsync(id);
    }
}
