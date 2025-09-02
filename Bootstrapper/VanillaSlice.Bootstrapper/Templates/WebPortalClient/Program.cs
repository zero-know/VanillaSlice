using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using {{ProjectName}}.Client.Shared;
using {{ProjectName}}.Client.Shared.Extensions;
using {{ProjectName}}.Framework;
using {{ProjectName}}.WebPortal.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddClientSideFeatureServices();

builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
builder.Services.AddTransient<CookieHandler>();

builder.Services.AddHttpClient<BaseHttpClient, HttpCookieClient>("ServerAPI", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
}).AddHttpMessageHandler<CookieHandler>();

await builder.Build().RunAsync();
