using {{RootNamespace}}.MauiNativeApp.ViewModels.__ComponentPrefix__;

namespace {{RootNamespace}}.MauiNativeApp.Views.__ComponentPrefix__;

public partial class __ComponentPrefix__FormPage : ContentPage
{
    public __ComponentPrefix__FormPage(__ComponentPrefix__FormPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}