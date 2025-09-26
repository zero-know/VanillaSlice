using ZKnow.VanillaStudio.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add controller support for API endpoints
builder.Services.AddControllers();


// Register project generation services
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.ProjectGenerationService>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.EnhancedProjectGenerationService>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.ProjectTemplateService>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.FrameworkCoreGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.HttpClientGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.SliceFactoryProjectGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.TemplateEngineService>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.TemplateBasedFrameworkCoreGenerator>();

// Register new template-based generators
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.ServerDataGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.CommonProjectGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.TemplateBasedServerDataGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.TemplateBasedCommonGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.PlatformProjectsGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.InfrastructureProjectsGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.WebPortalProjectsGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.HybridAppProjectsGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.MauiNativeAppProjectsGenerator>();
builder.Services.AddScoped<ZKnow.VanillaStudio.Services.ProjectValidationService>();

var app = builder.Build();

app.UseHttpsRedirection();

// Serve static files from wwwroot/downloads
app.UseStaticFiles();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map API controllers
app.MapControllers();

app.Run();
