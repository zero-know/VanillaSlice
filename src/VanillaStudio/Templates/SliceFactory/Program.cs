using Microsoft.EntityFrameworkCore;
using {{RootNamespace}}.SliceFactory.Components;
using {{RootNamespace}}.SliceFactory.Data;
using {{RootNamespace}}.SliceFactory.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
if (!Directory.Exists("DataFiles"))
{
    Directory.CreateDirectory("DataFiles");
}

// Add Entity Framework
builder.Services.AddDbContext<SliceFactoryDbContext>(options =>
    options.UseSqlite("Data Source=DataFiles/slicefactory.db"));

// Add pluralization service
builder.Services.AddScoped<PluralizationService>();

// Add template engine service
builder.Services.AddScoped<TemplateEngineService>();

// Add feature management service
builder.Services.AddScoped<FeatureManagementService>();

// Add registration management service
builder.Services.AddScoped<RegistrationManagementService>();

// Add navigation management service
builder.Services.AddScoped<NavigationManagementService>();

// Add placement guidance service
builder.Services.AddScoped<PlacementGuidanceService>();

// Add path detection service
builder.Services.AddScoped<PathDetectionService>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SliceFactoryDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
