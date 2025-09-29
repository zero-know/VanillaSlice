using Microsoft.Extensions.Logging;
using {{ProjectName}}.ClientShared;
using {{ProjectName}}.ClientShared.Extensions;
using {{ProjectName}}.Framework;
using {{ProjectName}}.MauiNativeApp.Services;
using {{ProjectName}}.MauiNativeApp.Views;
using {{ProjectName}}.MauiNativeApp.features.Products;
using CommunityToolkit.Maui;

namespace {{ProjectName}}.MauiNativeApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<ProductListPageViewModel>();
        builder.Services.AddTransient<ProductFormPageViewModel>();

        // Register Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<Views.Products.ProductListPage>();
        builder.Services.AddTransient<Views.Products.ProductFormPage>();

        // Add Client Services
        builder.Services.AddClientSideFeatureServices();
        builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
        builder.Services.AddHttpClient<BaseHttpClient, HttpTokenClient>("ServerAPI", client =>
        {
#if DEBUG
            client.BaseAddress = new Uri("https://localhost:7202");
#else
            client.BaseAddress = new Uri("https://localhost:7202");
#endif
        });

#if DEBUG
        builder.Services.AddLogging(logging =>
        {
            logging.AddDebug();
        });
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}