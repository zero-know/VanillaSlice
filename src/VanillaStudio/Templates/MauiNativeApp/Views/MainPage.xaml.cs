using {{ProjectName}}.MauiNativeApp.ViewModels;

namespace {{ProjectName}}.MauiNativeApp.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}