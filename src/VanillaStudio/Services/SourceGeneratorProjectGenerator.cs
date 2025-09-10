using ZKnow.VanillaStudio.Models;
using System.Text;

namespace ZKnow.VanillaStudio.Services
{
    public class SourceGeneratorProjectGenerator
    {
        public List<GeneratedFile> GenerateCompleteSourceGeneratorProject(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // SourceGenerator.csproj
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.SourceGenerator/{config.ProjectName}.SourceGenerator.csproj",
                Content = GenerateSourceGeneratorProjectFile(config),
                Type = FileType.ProjectFile
            });

            // Main Index.razor page
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.SourceGenerator/Pages/Index.razor",
                Content = GenerateIndexRazorContent(config),
                Type = FileType.RazorComponent
            });

            // Index.razor.cs code-behind
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.SourceGenerator/Pages/Index.razor.cs",
                Content = GenerateIndexRazorCodeBehindContent(config),
                Type = FileType.CSharpCode
            });

            // Configuration profiles
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.SourceGenerator/appsettings.webportal-profile.json",
                Content = GenerateWebPortalProfileContent(config),
                Type = FileType.JsonConfig
            });

            // Program.cs
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.SourceGenerator/Program.cs",
                Content = GenerateSourceGeneratorProgramContent(config),
                Type = FileType.CSharpCode
            });

            // App.razor
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.SourceGenerator/App.razor",
                Content = GenerateSourceGeneratorAppRazorContent(config),
                Type = FileType.RazorComponent
            });

            // _Imports.razor
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Base/{config.ProjectName}.SourceGenerator/_Imports.razor",
                Content = GenerateSourceGeneratorImportsContent(config),
                Type = FileType.RazorComponent
            });

            return files;
        }

        private string GenerateSourceGeneratorProjectFile(ProjectConfiguration config)
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly.Server"" Version=""9.0.8"" />
    <PackageReference Include=""MudBlazor"" Version=""7.8.0"" />
    <PackageReference Include=""Newtonsoft.Json"" Version=""13.0.3"" />
  </ItemGroup>

</Project>";
        }

        private string GenerateIndexRazorContent(ProjectConfiguration config)
        {
            return @"@page ""/""
@using System.ComponentModel.DataAnnotations
@using Newtonsoft.Json

<PageTitle>Code Generator</PageTitle>

<MudContainer MaxWidth=""MaxWidth.Large"" Class=""mt-4"">
    <MudPaper Class=""pa-4"">
        <MudText Typo=""Typo.h4"" Class=""mb-4"">
            <MudIcon Icon=""Icons.Material.Filled.Code"" Class=""mr-2"" />
            Blazor Code Generator
        </MudText>

        <EditForm Model=""@formViewModel"" OnValidSubmit=""@GenerateCode"">
            <DataAnnotationsValidator />
            
            <MudGrid>
                <MudItem xs=""12"" md=""6"">
                    <MudTextField @bind-Value=""formViewModel.BasePath""
                                  Label=""Base Path""
                                  Required=""true""
                                  For=""@(() => formViewModel.BasePath)""
                                  HelperText=""Root directory for code generation"" />
                </MudItem>
                
                <MudItem xs=""12"" md=""6"">
                    <MudTextField @bind-Value=""formViewModel.DirectoryName""
                                  Label=""Directory Name""
                                  Required=""true""
                                  For=""@(() => formViewModel.DirectoryName)""
                                  HelperText=""Target directory name"" />
                </MudItem>
                
                <MudItem xs=""12"" md=""6"">
                    <MudTextField @bind-Value=""formViewModel.ComponentPrefix""
                                  Label=""Component Prefix""
                                  Required=""true""
                                  For=""@(() => formViewModel.ComponentPrefix)""
                                  HelperText=""Prefix for generated components"" />
                </MudItem>
                
                <MudItem xs=""12"" md=""6"">
                    <MudTextField @bind-Value=""formViewModel.NameSpace""
                                  Label=""Namespace""
                                  For=""@(() => formViewModel.NameSpace)""
                                  HelperText=""Target namespace"" />
                </MudItem>
                
                <MudItem xs=""12"" md=""6"">
                    <MudSelect @bind-Value=""formViewModel.PkType""
                               Label=""Primary Key Type""
                               Required=""true""
                               For=""@(() => formViewModel.PkType)"">
                        <MudSelectItem Value=""@(""int"")"">int</MudSelectItem>
                        <MudSelectItem Value=""@(""long"")"">long</MudSelectItem>
                        <MudSelectItem Value=""@(""string"")"">string</MudSelectItem>
                        <MudSelectItem Value=""@(""Guid"")"">Guid</MudSelectItem>
                    </MudSelect>
                </MudItem>
                
                <MudItem xs=""12"">
                    <MudText Typo=""Typo.h6"" Class=""mb-2"">Generation Options</MudText>
                    <MudCheckBox @bind-Checked=""formViewModel.GenerateListing""
                                 Label=""Generate Listing Components"" />
                    <MudCheckBox @bind-Checked=""formViewModel.GenerateForm""
                                 Label=""Generate Form Components"" />
                    <MudCheckBox @bind-Checked=""formViewModel.GenerateControllerAndClientService""
                                 Label=""Generate API Controllers and Client Services"" />
                </MudItem>
                
                <MudItem xs=""12"">
                    <MudButton ButtonType=""ButtonType.Submit""
                               Variant=""Variant.Filled""
                               Color=""Color.Primary""
                               StartIcon=""Icons.Material.Filled.Build""
                               Disabled=""@isGenerating"">
                        @if (isGenerating)
                        {
                            <MudProgressCircular Size=""Size.Small"" Indeterminate=""true"" Class=""mr-2"" />
                            <text>Generating...</text>
                        }
                        else
                        {
                            <text>Generate Code</text>
                        }
                    </MudButton>
                </MudItem>
            </MudGrid>
        </EditForm>

        @if (!string.IsNullOrEmpty(generationResult))
        {
            <MudAlert Severity=""Severity.Success"" Class=""mt-4"">
                @generationResult
            </MudAlert>
        }

        @if (!string.IsNullOrEmpty(errorMessage))
        {
            <MudAlert Severity=""Severity.Error"" Class=""mt-4"">
                @errorMessage
            </MudAlert>
        }
    </MudPaper>
</MudContainer>";
        }

        private string GenerateIndexRazorCodeBehindContent(ProjectConfiguration config)
        {
            return @"using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SourceGenerator.Pages
{
    public partial class Index
    {
        private FormViewModel formViewModel = new();
        private bool isGenerating = false;
        private string generationResult = string.Empty;
        private string errorMessage = string.Empty;

        private async Task GenerateCode()
        {
            isGenerating = true;
            generationResult = string.Empty;
            errorMessage = string.Empty;

            try
            {
                // Load configuration
                var configPath = Path.Combine(Directory.GetCurrentDirectory(), ""appsettings.webportal-profile.json"");
                var configJson = await File.ReadAllTextAsync(configPath);
                var config = JsonConvert.DeserializeObject<CodeConfig>(configJson);

                if (config == null)
                {
                    errorMessage = ""Failed to load configuration"";
                    return;
                }

                // Generate code based on configuration
                var generator = new CodeGenerator();
                var result = await generator.GenerateDynamicCode(formViewModel, config);

                generationResult = $""Code generation completed successfully! Generated {result.GeneratedFiles} files."";
            }
            catch (Exception ex)
            {
                errorMessage = $""Error during code generation: {ex.Message}"";
            }
            finally
            {
                isGenerating = false;
            }
        }
    }

    public class FormViewModel
    {
        [Required]
        public string? BasePath { get; set; }

        [Required]
        public string? DirectoryName { get; set; }

        public string NameSpace { get; set; } = string.Empty;

        [Required]
        public string? ComponentPrefix { get; set; }

        public bool GenerateListing { get; set; } = true;

        public bool GenerateControllerAndClientService { get; set; } = true;

        public bool GenerateForm { get; set; } = true;

        [Required]
        public string? PkType { get; set; } = ""int"";
    }

    public class CodeConfig
    {
        public List<CodeProfile> Profiles { get; set; } = new();
        public Dictionary<ProjectType, List<CodeFile>> ProjectFiles { get; set; } = new();
    }

    public class CodeProfile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string NameSpace { get; set; } = string.Empty;
        public int ParentId { get; set; }
        public ProjectType ProjectType { get; set; }
    }

    public class CodeFile
    {
        public string FileName { get; set; } = string.Empty;
        public int FileCategory { get; set; }
        public List<string> Content { get; set; } = new();
    }

    public enum ProjectType
    {
        ServiceContracts = 1,
        Controllers = 2,
        ClientShared = 3,
        RazorComponents = 4,
        ServerSideServices = 5
    }

    public class CodeGenerator
    {
        public async Task<GenerationResult> GenerateDynamicCode(FormViewModel form, CodeConfig config)
        {
            var result = new GenerationResult();
            
            // Implementation would go here
            await Task.Delay(1000); // Simulate work
            
            result.GeneratedFiles = 10;
            result.Success = true;
            
            return result;
        }
    }

    public class GenerationResult
    {
        public bool Success { get; set; }
        public int GeneratedFiles { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}";
        }

        private string GenerateWebPortalProfileContent(ProjectConfiguration config)
        {
            var json = @"{
  ""Profiles"": [
    {
      ""Id"": 1,
      ""Name"": ""ServiceContracts"",
      ""Path"": """ + config.ProjectName + @"\" + config.ProjectName + @".ServiceContracts\Modules"",
      ""NameSpace"": """ + config.RootNamespace + @".ServiceContracts.Modules"",
      ""ParentId"": 0,
      ""ProjectType"": 1
    },
    {
      ""Id"": 2,
      ""Name"": ""Controllers"",
      ""Path"": """ + config.ProjectName + @"\WebPortal\" + config.ProjectName + @".WebPortal\Controllers"",
      ""NameSpace"": """ + config.RootNamespace + @".WebPortal.Controllers"",
      ""ParentId"": 0,
      ""ProjectType"": 2
    },
    {
      ""Id"": 3,
      ""Name"": ""ClientShared"",
      ""Path"": """ + config.ProjectName + @"\" + config.ProjectName + @".Client.Shared\Modules"",
      ""NameSpace"": """ + config.RootNamespace + @".Client.Shared.Modules"",
      ""ParentId"": 0,
      ""ProjectType"": 3
    },
    {
      ""Id"": 4,
      ""Name"": ""RazorComponents"",
      ""Path"": """ + config.ProjectName + @"\WebPortal\" + config.ProjectName + @".WebPortal.Razor\Modules"",
      ""NameSpace"": """ + config.RootNamespace + @".WebPortal.Razor.Modules"",
      ""ParentId"": 0,
      ""ProjectType"": 4
    },
    {
      ""Id"": 5,
      ""Name"": ""ServerSideServices"",
      ""Path"": """ + config.ProjectName + @"\" + config.ProjectName + @".Server.DataServices\Modules"",
      ""NameSpace"": """ + config.RootNamespace + @".Server.DataServices.Modules"",
      ""ParentId"": 0,
      ""ProjectType"": 5
    }
  ],
  ""ProjectFiles"": {
    ""ServiceContracts"": [
      {
        ""FileName"": ""Listing\I##ComponentPrefix##ListingDataService.cs"",
        ""FileCategory"": 1,
        ""Content"": [
          ""using Framework.Core;"",
          ""namespace " + config.RootNamespace + @".ServiceContracts.Modules.##moduleNamespace##;"",
          ""public interface I##ComponentPrefix##ListingDataService :"",
          ""\tIListingDataService<##ComponentPrefix##ListingBusinessObject, ##ComponentPrefix##FilterBusinessObject>"",
          ""{"",
          ""\t//Add any custom methods here"",
          ""}""
        ]
      }
    ],
    ""Controllers"": [
      {
        ""FileName"": ""##ComponentPrefix##sListingController.cs"",
        ""FileCategory"": 1,
        ""Content"": [
          ""using Microsoft.AspNetCore.Mvc;"",
          ""using " + config.RootNamespace + @".ServiceContracts.Modules.##moduleNamespace##;"",
          ""namespace " + config.RootNamespace + @".WebPortal.Controllers;"",
          ""[ApiController]"",
          ""[Route(\""api/[controller]\"")]"",
          ""public class ##ComponentPrefix##sListingController : ControllerBase"",
          ""{"",
          ""\tprivate readonly I##ComponentPrefix##ListingDataService _service;"",
          ""\tpublic ##ComponentPrefix##sListingController(I##ComponentPrefix##ListingDataService service)"",
          ""\t{"",
          ""\t\t_service = service;"",
          ""\t}"",
          ""\t[HttpGet(\""GetPaginatedItems\"")]"",
          ""\tpublic async Task<IActionResult> GetPaginatedItems([FromQuery] ##ComponentPrefix##FilterBusinessObject filter)"",
          ""\t{"",
          ""\t\tvar result = await _service.GetPaginatedItems(filter);"",
          ""\t\treturn Ok(result);"",
          ""\t}"",
          ""}""
        ]
      }
    ]
  }
}";
            return json;
        }

        private string GenerateSourceGeneratorProgramContent(ProjectConfiguration config)
        {
            return @"using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(""/Error"");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage(""/_Host"");

app.Run();";
        }

        private string GenerateSourceGeneratorAppRazorContent(ProjectConfiguration config)
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Code Generator</title>
    <base href=""~/"" />
    <link href=""https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap"" rel=""stylesheet"" />
    <link href=""_content/MudBlazor/MudBlazor.min.css"" rel=""stylesheet"" />
</head>
<body>
    <component type=""typeof(App)"" render-mode=""ServerPrerendered"" />
    <script src=""_framework/blazor.server.js""></script>
    <script src=""_content/MudBlazor/MudBlazor.min.js""></script>
</body>
</html>";
        }

        private string GenerateSourceGeneratorImportsContent(ProjectConfiguration config)
        {
            return @"@using System.Net.Http
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using MudBlazor
@using SourceGenerator
@using SourceGenerator.Shared";
        }
    }
}
