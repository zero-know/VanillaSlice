using {{ProjectName}}.MauiNativeApp.Views.Products;

namespace {{ProjectName}}.MauiNativeApp.Views;

public partial class AppShellFlyout : Shell
{
    public AppShellFlyout()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(ProductFormPage), typeof(ProductFormPage));
        Routing.RegisterRoute("ProductDetails", typeof(ProductFormPage));
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
        await DisplayAlert("Settings", "Settings functionality coming soon!", "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (result)
        {
            // Implement logout logic here
            await DisplayAlert("Logout", "Logout functionality coming soon!", "OK");
        }
    }
}