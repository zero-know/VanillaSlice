using {{ProjectName}}.MauiNativeApp.Views.Products;

namespace {{ProjectName}}.MauiNativeApp.Views;

public partial class AppShellTabs : Shell
{
    public AppShellTabs()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(ProductFormPage), typeof(ProductFormPage));
        Routing.RegisterRoute("ProductDetails", typeof(ProductFormPage));
    }
}