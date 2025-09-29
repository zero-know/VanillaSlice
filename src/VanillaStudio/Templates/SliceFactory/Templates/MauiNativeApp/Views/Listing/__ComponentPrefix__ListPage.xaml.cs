using {{RootNamespace}}.MauiNativeApp.ViewModels.__ComponentPrefix__;

namespace {{RootNamespace}}.MauiNativeApp.Views.__ComponentPrefix__;

public partial class __ComponentPrefix__ListPage : ContentPage
{
    public __ComponentPrefix__ListPage(__ComponentPrefix__ListPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}