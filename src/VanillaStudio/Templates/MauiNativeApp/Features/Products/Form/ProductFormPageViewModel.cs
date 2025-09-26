using {{ProjectName}}.ClientShared.Features.Products;
using {{ProjectName}}.Common.Enums;
using {{ProjectName}}.ServiceContracts.Features.Products;
using {{ProjectName}}.MauiShared; 

namespace {{ProjectName}}.MauiNativeApp.Features.Products;
public partial class ProductFormPageViewModel : FormBaseMaui<ProductFormBusinessModel, ProductFormViewModel, string, IProductFormDataService>
{
    public ProductFormPageViewModel(IServiceScopeFactory scopeFactory, string id) : base(scopeFactory, id)
    {
    }
}