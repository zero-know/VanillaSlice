using {{ProjectName}}.MauiNativeApp.Views;

namespace {{ProjectName}}.MauiNativeApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        {{#if (eq NavigationType "Tabs")}}
        MainPage = new AppShellTabs();
        {{/if}}

        {{#if (eq NavigationType "Flyout")}}
        MainPage = new AppShellFlyout();
        {{/if}}
    }
}