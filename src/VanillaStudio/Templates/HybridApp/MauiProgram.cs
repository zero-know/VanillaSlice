using Microsoft.Extensions.Logging;
using {{ProjectName}}.Client.Shared;
using {{ProjectName}}.Client.Shared.Extensions;
using {{ProjectName}}.Framework;
using {{ProjectName}}.HybridApp.Helpers;

namespace {{ProjectName}}.HybridApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<TokenHandler>();
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
#endif

            return builder.Build();
        }
    }
}
