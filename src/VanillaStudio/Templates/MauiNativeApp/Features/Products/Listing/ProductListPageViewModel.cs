using {{ProjectName}}.ClientShared.Features.Products;
using {{ProjectName}}.Common.Enums;
using {{ProjectName}}.MauiShared;
using {{ProjectName}}.ServiceContracts.Features.Products;
using System.Windows.Input;

namespace {{ProjectName}}.MauiNativeApp.Features.Products;

public partial class ProductListPageViewModel : ListingBaseMaui<ProductListingViewModel, ProductListingBusinessModel,
                                ProductFilterViewModel, ProductFilterBusinessModel, IProductListingDataService>
{
    public ProductListPageViewModel(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
        //Title = "Products";
    }

    private ICommand? _addProductCommand;
    public ICommand AddProductCommand
    {
        get
        {
            return _addProductCommand ??= new Command(async () =>
            {
                await Shell.Current.GoToAsync("//products/form/new");
            });
        }
    }

    private ICommand? _editProductCommand;
    public ICommand EditProductCommand
    {
        get
        {
            return _editProductCommand ??= new Command<ProductListingViewModel>(async (product) =>
            {
                if (product?.Id != null)
                {
                    await Shell.Current.GoToAsync($"//products/form/{product.Id}");
                }
            });
        }
    }
}