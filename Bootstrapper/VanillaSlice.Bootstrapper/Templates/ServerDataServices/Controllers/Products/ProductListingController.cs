using Microsoft.AspNetCore.Mvc;
using {{ProjectName}}.Framework;
using {{ProjectName}}.ServiceContracts.Features.Products;

namespace {{ProjectName}}.Server.DataServices.Controllers.Products;

[ApiController, Route("api/[controller]/[action]")]
public class ProductListingController : ControllerBase, IProductListingDataService
{
    private readonly IProductListingDataService dataService;

    public ProductListingController(IProductListingDataService dataService)
    {
        this.dataService = dataService;
    }

    [HttpGet]
    public async Task<PagedDataList<ProductListingBusinessModel>> GetPaginatedItemsAsync([FromQuery] ProductFilterBusinessModel filterViewModel)
    {
        return await dataService.GetPaginatedItemsAsync(filterViewModel);
    }
}