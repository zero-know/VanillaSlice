namespace {{ProjectName}}.MauiNativeApp.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
    }

    private async void OnLearnMoreClicked(object sender, EventArgs e)
    {
        await Launcher.OpenAsync("https://github.com/dotnet/maui");
    }
}