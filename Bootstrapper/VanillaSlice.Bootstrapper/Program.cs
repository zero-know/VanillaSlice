using VanillaSlice.Bootstrapper.Components; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add controller support for API endpoints
builder.Services.AddControllers();

 
// Register project generation services
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.ProjectGenerationService>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.EnhancedProjectGenerationService>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.ProjectTemplateService>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.FrameworkCoreGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.HttpClientGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.SliceFactoryProjectGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.TemplateEngineService>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.TemplateBasedFrameworkCoreGenerator>();

// Register new template-based generators
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.ServerDataGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.CommonProjectGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.TemplateBasedServerDataGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.TemplateBasedCommonGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.PlatformProjectsGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.InfrastructureProjectsGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.WebPortalProjectsGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.HybridAppProjectsGenerator>();
builder.Services.AddScoped<VanillaSlice.Bootstrapper.Services.ProjectValidationService>();

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
