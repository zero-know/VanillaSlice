using {{ProjectName}}.MauiNativeApp.Features.Products;

namespace {{ProjectName}}.MauiNativeApp.Views.Products;

public partial class ProductFormPage : ContentPage
{
    public ProductFormPage(IServiceScopeFactory scopeFactory, string id)
    {
        InitializeComponent();
        BindingContext = new ProductFormPageViewModel(scopeFactory, id);
    }
}