using {{ProjectName}}.MauiNativeApp.Features.Products;

namespace {{ProjectName}}.MauiNativeApp.Views.Products;
public partial class ProductListPage : ContentPage
{ 
    private readonly IServiceScopeFactory scopeFactory;

    public ProductListPage(IServiceScopeFactory scopeFactory)
    { 
        this.scopeFactory = scopeFactory;

        InitializeComponent();

        var scope = scopeFactory.CreateScope();
        var viewModel = scope.ServiceProvider.GetRequiredService<ProductListPageViewModel>();
         
        BindingContext = viewModel; 
    }
     
}