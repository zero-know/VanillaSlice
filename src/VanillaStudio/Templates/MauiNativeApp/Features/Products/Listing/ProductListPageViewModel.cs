using {{ProjectName}}.ClientShared.Features.Products;
using {{ProjectName}}.Common.Enums;
using {{ProjectName}}.MauiShared; 
using {{ProjectName}}.ServiceContracts.Features.Products;
namespace {{ProjectName}}.MauiNativeApp.ViewModels.Products;

public partial class ProductListPageViewModel : ListingBaseMaui<ProductListingViewModel, ProductListingBusinessModel,
                                ProductFilterViewModel, ProductFilterBusinessModel, IProductListingDataService>
{
    public ProductListPageViewModel(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        //Title = "Products";
    }
}