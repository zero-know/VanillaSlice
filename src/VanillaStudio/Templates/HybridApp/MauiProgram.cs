using Microsoft.Extensions.Logging;
using {{ProjectName}}.ClientShared;
using {{ProjectName}}.ClientShared.Extensions;
using {{ProjectName}}.Framework;
using {{ProjectName}}.HybridApp.Services;
{{#if (eq UIFramework "FluentUI")}}
using Microsoft.FluentUI.AspNetCore.Components;
{{/if}}
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
            builder.Services.AddBlazorWebViewDeveloperTools();
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

            // UI Framework Services
            {{#if (eq UIFramework "FluentUI")}}
            builder.Services.AddFluentUIComponents();
            {{/if}}

            {{#if (eq UIFramework "MudBlazor")}}
            builder.Services.AddMudServices();
            {{/if}}

            {{#if (eq UIFramework "Radzen")}}
            builder.Services.AddRadzenComponents();
            {{/if}}

            return builder.Build();
        }
    }
}
