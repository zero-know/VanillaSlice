using Microsoft.AspNetCore.Mvc;
using {{ProjectName}}.ServiceContracts.Features.Products;

namespace {{ProjectName}}.Server.DataServices.Controllers.Products;

[ApiController, Route("api/[controller]")]
public class ProductFormController : ControllerBase, IProductFormDataService
{
    private readonly IProductFormDataService dataService;

    public ProductFormController(IProductFormDataService dataService)
    {
        this.dataService = dataService;
    }

    [HttpPost]
    public async Task<string> CreateAsync([FromBody] ProductFormBusinessModel formBusinessObject)
    {
        return await dataService.CreateAsync(formBusinessObject);
    }

    [HttpPut("{id}")]
    public async Task<string> UpdateAsync(string id, [FromBody] ProductFormBusinessModel formBusinessObject)
    {
        return await dataService.UpdateAsync(id, formBusinessObject);
    }

    [HttpGet("{id}")]
    public async Task<ProductFormBusinessModel> GetByIdAsync(string id)
    {
        return await dataService.GetByIdAsync(id);
    }

    [HttpDelete("{id}")]
    public async Task<int> DeleteAsync(string id)
    {
        return await dataService.DeleteAsync(id);
    }
}
