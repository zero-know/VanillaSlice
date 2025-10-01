using {{RootNamespace}}.MauiNativeApp.ViewModels.__moduleNamespace__;

namespace {{RootNamespace}}.MauiNativeApp.Views.__moduleNamespace__;

public partial class __ComponentPrefix__ListPage : ContentPage
{
    public __ComponentPrefix__ListPage(__ComponentPrefix__ListPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}