using CommunityToolkit.Mvvm.ComponentModel;

namespace {{ProjectName}}.MauiNativeApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    string title = string.Empty;
}