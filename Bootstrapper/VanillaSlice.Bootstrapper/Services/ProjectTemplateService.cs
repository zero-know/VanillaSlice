using VanillaSlice.Bootstrapper.Models;
using VanillaSlice.Bootstrapper.Extensions;
using System.Text;

namespace VanillaSlice.Bootstrapper.Services
{
    public class ProjectTemplateService
    {
        public List<GeneratedFile> GenerateMainProjectStructure(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // Generate WebPortal project
            files.AddRange(GenerateWebPortalProject(config));

            // Generate Client project if Auto rendering mode
            if (config.RenderingMode == RenderingMode.Auto)
            {
                files.AddRange(GenerateClientProject(config));
            }

            // Generate Razor library if Common Library strategy
            if (config.ComponentStrategy == ComponentStrategy.CommonLibrary)
            {
                files.AddRange(GenerateRazorLibraryProject(config));
            }

            // MAUI project generation is now handled in EnhancedProjectGenerationService

            // Generate Aspire projects if enabled
            if (config.UseAspireOrchestration)
            {
                files.AddRange(GenerateAspireProjects(config));
            }

            return files;
        }

        private List<GeneratedFile> GenerateWebPortalProject(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // WebPortal.csproj
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}/WebPortal/{config.ProjectName}.WebPortal/{config.ProjectName}.WebPortal.csproj",
                Content = GenerateWebPortalProjectFile(config),
                Type = FileType.ProjectFile
            });

            // Program.cs
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}/WebPortal/{config.ProjectName}.WebPortal/Program.cs",
                Content = GenerateWebPortalProgramCs(config),
                Type = FileType.CSharpCode
            });

            // App.razor
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}/WebPortal/{config.ProjectName}.WebPortal/Components/App.razor",
                Content = GenerateAppRazor(config),
                Type = FileType.RazorComponent
            });

            // Routes.razor
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}/WebPortal/{config.ProjectName}.WebPortal/Components/Routes.razor",
                Content = GenerateRoutesRazor(config),
                Type = FileType.RazorComponent
            });

            // Home page
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}/WebPortal/{config.ProjectName}.WebPortal/Components/Pages/Home.razor",
                Content = GenerateHomePageRazor(config),
                Type = FileType.RazorComponent
            });

            // Layout components
            files.AddRange(GenerateLayoutComponents(config));

            return files;
        }

        private string GenerateWebPortalProjectFile(ProjectConfiguration config)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<Project Sdk=""Microsoft.NET.Sdk.Web"">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine("    <TargetFramework>net9.0</TargetFramework>");
            sb.AppendLine("    <Nullable>enable</Nullable>");
            sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine(@"    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly.Server"" Version=""9.0.8"" />");

            if (config.IncludeAuthentication)
            {
                sb.AppendLine(@"    <PackageReference Include=""Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore"" Version=""9.0.8"" />");
                sb.AppendLine(@"    <PackageReference Include=""Microsoft.AspNetCore.Identity.EntityFrameworkCore"" Version=""9.0.8"" />");
            }

            if (config.IncludeDatabase)
            {
                switch (config.DatabaseProvider)
                {
                    case DatabaseProvider.SqlServer:
                        sb.AppendLine(@"    <PackageReference Include=""Microsoft.EntityFrameworkCore.SqlServer"" Version=""9.0.8"" />");
                        break;
                    case DatabaseProvider.SQLite:
                        sb.AppendLine(@"    <PackageReference Include=""Microsoft.EntityFrameworkCore.Sqlite"" Version=""9.0.8"" />");
                        break;
                    case DatabaseProvider.PostgreSQL:
                        sb.AppendLine(@"    <PackageReference Include=""Npgsql.EntityFrameworkCore.PostgreSQL"" Version=""9.0.8"" />");
                        break;
                }
                sb.AppendLine(@"    <PackageReference Include=""Microsoft.EntityFrameworkCore.Tools"" Version=""9.0.8"">");
                sb.AppendLine("      <PrivateAssets>all</PrivateAssets>");
                sb.AppendLine("      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>");
                sb.AppendLine("    </PackageReference>");
            }

            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine(@"    <ProjectReference Include=""..\..\..\Framework\Framework.Core\Framework.Core.csproj"" />");
            sb.AppendLine(@"    <ProjectReference Include=""..\..\..\Platform\Platform.Common\Platform.Common.csproj"" />");

            if (config.IncludeDatabase)
            {
                sb.AppendLine(@"    <ProjectReference Include=""..\..\..\Platform\Platform.Server.Data\Platform.Server.Data.csproj"" />");
            }

            if (config.RenderingMode == RenderingMode.Auto)
            {
                sb.AppendLine($@"    <ProjectReference Include=""..\{config.ProjectName}.WebPortal.Client\{config.ProjectName}.WebPortal.Client.csproj"" />");
            }

            if (config.ComponentStrategy == ComponentStrategy.CommonLibrary)
            {
                sb.AppendLine($@"    <ProjectReference Include=""..\{config.ProjectName}.WebPortal.Razor\{config.ProjectName}.WebPortal.Razor.csproj"" />");
            }

            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("</Project>");

            return sb.ToString();
        }

        private string GenerateWebPortalProgramCs(ProjectConfiguration config)
        {
            var sb = new StringBuilder();
            sb.AppendLine("var builder = WebApplication.CreateBuilder(args);");
            sb.AppendLine();
            sb.AppendLine("// Add services to the container.");

            switch (config.RenderingMode)
            {
                case RenderingMode.Auto:
                    sb.AppendLine("builder.Services.AddRazorComponents()");
                    sb.AppendLine("    .AddInteractiveServerComponents()");
                    sb.AppendLine("    .AddInteractiveWebAssemblyComponents();");
                    break;
                case RenderingMode.ServerOnly:
                    sb.AppendLine("builder.Services.AddRazorComponents()");
                    sb.AppendLine("    .AddInteractiveServerComponents();");
                    break;
                case RenderingMode.StaticSSR:
                    sb.AppendLine("builder.Services.AddRazorComponents();");
                    break;
            }

            if (config.IncludeAuthentication)
            {
                sb.AppendLine();
                sb.AppendLine("// Add authentication services");
                sb.AppendLine("builder.Services.AddCascadingAuthenticationState();");
                sb.AppendLine("// Add Identity services here if needed");
            }

            if (config.IncludeDatabase)
            {
                sb.AppendLine();
                sb.AppendLine("// Add database services");
                sb.AppendLine($"var connectionString = builder.Configuration.GetConnectionString(\"{config.ConnectionStringName}\") ?? throw new InvalidOperationException(\"Connection string '{config.ConnectionStringName}' not found.\");");
                sb.AppendLine("builder.Services.AddDbContext<Platform.Server.Data.EF.AppDbContext>(options =>");

                switch (config.DatabaseProvider)
                {
                    case DatabaseProvider.SqlServer:
                        sb.AppendLine("    options.UseSqlServer(connectionString));");
                        break;
                    case DatabaseProvider.SQLite:
                        sb.AppendLine("    options.UseSqlite(connectionString));");
                        break;
                    case DatabaseProvider.PostgreSQL:
                        sb.AppendLine("    options.UseNpgsql(connectionString));");
                        break;
                }
            }

            sb.AppendLine();
            sb.AppendLine("var app = builder.Build();");
            sb.AppendLine();
            sb.AppendLine("// Configure the HTTP request pipeline.");
            sb.AppendLine("if (!app.Environment.IsDevelopment())");
            sb.AppendLine("{");
            sb.AppendLine("    app.UseExceptionHandler(\"/Error\", createScopeForErrors: true);");
            sb.AppendLine("    app.UseHsts();");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("app.UseHttpsRedirection();");
            sb.AppendLine("app.UseStaticFiles();");
            sb.AppendLine("app.UseAntiforgery();");
            sb.AppendLine();
            sb.AppendLine("app.MapStaticAssets();");

            switch (config.RenderingMode)
            {
                case RenderingMode.Auto:
                    sb.AppendLine("app.MapRazorComponents<App>()");
                    sb.AppendLine("    .AddInteractiveServerRenderMode()");
                    sb.AppendLine("    .AddInteractiveWebAssemblyRenderMode()");
                    sb.AppendLine($"    .AddAdditionalAssemblies(typeof({config.ProjectName}.WebPortal.Client._Imports).Assembly);");
                    break;
                case RenderingMode.ServerOnly:
                    sb.AppendLine("app.MapRazorComponents<App>()");
                    sb.AppendLine("    .AddInteractiveServerRenderMode();");
                    break;
                case RenderingMode.StaticSSR:
                    sb.AppendLine("app.MapRazorComponents<App>();");
                    break;
            }

            sb.AppendLine();
            sb.AppendLine("app.Run();");

            return sb.ToString();
        }

        private string GenerateAppRazor(ProjectConfiguration config)
        {
            return @"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <base href=""/"" />
    <link rel=""stylesheet"" href=""@Assets[""lib/bootstrap/dist/css/bootstrap.min.css""]"" />
    <link rel=""stylesheet"" href=""@Assets[""app.css""]"" />
    <link rel=""stylesheet"" href=""@Assets[""" + config.ProjectName + @".WebPortal.styles.css""]"" />
    <ImportMap />
    <link rel=""icon"" type=""image/png"" href=""favicon.png"" />
    <HeadOutlet @rendermode=""PageRenderMode"" />
</head>

<body>
    <Routes @rendermode=""PageRenderMode"" />
    <script src=""_framework/blazor.web.js""></script>
</body>

</html>";
        }

        private string GenerateRoutesRazor(ProjectConfiguration config)
        {
            return @"<Router AppAssembly=""@typeof(Program).Assembly""" +
                   (config.RenderingMode == RenderingMode.Auto ? $" AdditionalAssemblies=\"new[] {{ typeof({config.ProjectName}.WebPortal.Client._Imports).Assembly }}\"" : "") +
                   @">
    <Found Context=""routeData"">
        <RouteView RouteData=""@routeData"" DefaultLayout=""@typeof(Layout.MainLayout)"" />
        <FocusOnNavigate RouteData=""@routeData"" Selector=""h1"" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout=""@typeof(Layout.MainLayout)"">
            <p role=""alert"">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>";
        }

        private string GenerateHomePageRazor(ProjectConfiguration config)
        {
            return $@"@page ""/""

<PageTitle>{config.ProjectName}</PageTitle>

<h1>Welcome to {config.ProjectName}!</h1>

<p>
    This is your new Blazor application generated by the Project Bootstrapper.
</p>

<div class=""alert alert-info"">
    <h4>Project Configuration:</h4>
    <ul>
        <li><strong>Platform:</strong> {config.PlatformType.GetDisplayName()}</li>
        <li><strong>Rendering Mode:</strong> {config.RenderingMode.GetDisplayName()}</li>
        <li><strong>Component Strategy:</strong> {config.ComponentStrategy.GetDisplayName()}</li>
        <li><strong>Database:</strong> {config.DatabaseProvider.GetDisplayName()}</li>
    </ul>
</div>";
        }

        private List<GeneratedFile> GenerateLayoutComponents(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // MainLayout.razor
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}/WebPortal/{config.ProjectName}.WebPortal/Components/Layout/MainLayout.razor",
                Content = @"@inherits LayoutView
@namespace " + config.RootNamespace + @".WebPortal.Layout

<div class=""page"">
    <div class=""sidebar"">
        <NavMenu />
    </div>

    <main>
        <div class=""top-row px-4"">
            <a href=""https://learn.microsoft.com/aspnet/core/"" target=""_blank"">About</a>
        </div>

        <article class=""content px-4"">
            @Body
        </article>
    </main>
</div>

<div id=""blazor-error-ui"">
    An unhandled error has occurred.
    <a href="""" class=""reload"">Reload</a>
    <a class=""dismiss"">🗙</a>
</div>",
                Type = FileType.RazorComponent
            });

            // NavMenu.razor
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}/WebPortal/{config.ProjectName}.WebPortal/Components/Layout/NavMenu.razor",
                Content = @"<div class=""top-row ps-3 navbar navbar-dark"">
    <div class=""container-fluid"">
        <a class=""navbar-brand"" href="""">" + config.ProjectName + @"</a>
    </div>
</div>

<input type=""checkbox"" title=""Navigation menu"" class=""navbar-toggler"" />

<div class=""nav-scrollable"" onclick=""document.querySelector('.navbar-toggler').click()"">
    <nav class=""nav flex-column"">
        <div class=""nav-item px-3"">
            <NavLink class=""nav-link"" href="""" Match=""NavLinkMatch.All"">
                <span class=""bi bi-house-door-fill-nav-menu"" aria-hidden=""true""></span> Home
            </NavLink>
        </div>
    </nav>
</div>",
                Type = FileType.RazorComponent
            });

            return files;
        }

        private List<GeneratedFile> GenerateClientProject(ProjectConfiguration config)
        {
            // This will be implemented for Auto rendering mode
            return new List<GeneratedFile>();
        }

        private List<GeneratedFile> GenerateRazorLibraryProject(ProjectConfiguration config)
        {
            // This will be implemented for Common Library strategy
            return new List<GeneratedFile>();
        }



        private List<GeneratedFile> GenerateAspireProjects(ProjectConfiguration config)
        {
            // This will be implemented for Aspire orchestration
            return new List<GeneratedFile>();
        }
    }
}
