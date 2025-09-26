using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace {{ProjectName}}.MauiNativeApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    public MainViewModel()
    {
        Title = "{{ProjectName}}";
    }

    [RelayCommand]
    async Task GoToProducts()
    {
        await Shell.Current.GoToAsync("//ProductListPage");
    }

    [RelayCommand]
    async Task GoToAbout()
    {
        await Shell.Current.GoToAsync("//AboutPage");
    }
}