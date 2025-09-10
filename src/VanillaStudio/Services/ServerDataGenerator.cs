using ZKnow.VanillaStudio.Models;

namespace ZKnow.VanillaStudio.Services
{
    public class ServerDataGenerator
    {
        public List<GeneratedFile> GenerateServerDataProject(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // Project file
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Common/{config.ProjectName}.Server.Data/{config.ProjectName}.Server.Data.csproj",
                Content = GenerateServerDataProjectFile(config),
                Type = FileType.ProjectFile
            });

            // AppDbContext
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Common/{config.ProjectName}.Server.Data/EF/AppDbContext.cs",
                Content = GenerateAppDbContextContent(config),
                Type = FileType.CSharpCode
            });

            // ApplicationUser (only if authentication is enabled)
            if (config.IncludeAuthentication)
            {
                files.Add(new GeneratedFile
                {
                    RelativePath = $"{config.ProjectName}.Common/{config.ProjectName}.Server.Data/EF/ApplicationUser.cs",
                    Content = GenerateApplicationUserContent(config),
                    Type = FileType.CSharpCode
                });
            }

            // Product entity (only if sample data is enabled)
            if (config.IncludeSampleData)
            {
                files.Add(new GeneratedFile
                {
                    RelativePath = $"{config.ProjectName}.Common/{config.ProjectName}.Server.Data/EF/Product.cs",
                    Content = GenerateProductContent(config),
                    Type = FileType.CSharpCode
                });
            }

            // ServerSideListingDataService
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Common/{config.ProjectName}.Server.Data/Services/ListingServerDataService.cs",
                Content = GenerateServerSideListingDataServiceContent(config),
                Type = FileType.CSharpCode
            });

            return files;
        }

        private string GenerateServerDataProjectFile(ProjectConfiguration config)
        {
            var identityPackage = config.IncludeAuthentication 
                ? "    <PackageReference Include=\"Microsoft.AspNetCore.Identity.EntityFrameworkCore\" Version=\"9.0.8\" />\n"
                : "";

            return $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
{identityPackage}    <PackageReference Include=""Microsoft.EntityFrameworkCore"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.Design"" Version=""9.0.8"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.SqlServer"" Version=""9.0.8"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""..\..\{config.ProjectName}.Base\{config.ProjectName}.Framework\{config.ProjectName}.Framework.csproj"" />
    <ProjectReference Include=""..\{config.ProjectName}.Common\{config.ProjectName}.Common.csproj"" />
  </ItemGroup>
</Project>";
        }

        private string GenerateAppDbContextContent(ProjectConfiguration config)
        {
            var identityUsing = config.IncludeAuthentication 
                ? "using Microsoft.AspNetCore.Identity.EntityFrameworkCore;\n"
                : "";

            var baseClass = config.IncludeAuthentication 
                ? "IdentityDbContext<ApplicationUser>"
                : "DbContext";

            var productDbSet = config.IncludeSampleData 
                ? "\n        // Sample DbSet - replace with your actual entities\n        public DbSet<Product> Products { get; set; }\n"
                : "";

            var productConfiguration = config.IncludeSampleData 
                ? @"
            // Configure your entities here
            modelBuilder.Entity<Product>(entity =>
            { 
                entity.Property(e => e.Price).HasColumnType(""decimal(18,2)"");
            });"
                : "";

            return $@"{identityUsing}using Microsoft.EntityFrameworkCore; 

namespace {config.ProjectName}.Server.Data.EF;
    public class AppDbContext : {baseClass}
{{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {{
        }}{productDbSet}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {{
            base.OnModelCreating(modelBuilder);{productConfiguration}
        }}
    }} 
 ";
        }

        private string GenerateApplicationUserContent(ProjectConfiguration config)
        {
            return $@"using Microsoft.AspNetCore.Identity;

namespace {config.ProjectName}.Server.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{{
}}";
        }

        private string GenerateProductContent(ProjectConfiguration config)
        {
            return $@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {config.ProjectName}.Common.Enums;

namespace {config.ProjectName}.Server.Data.EF
{{
    /// <summary>
    /// sample entity class representing a product in the database.
    /// </summary>
    public class Product
    {{
        [Key, MaxLength(100)]
        public string Id {{ get; set; }} = string.Empty;

        [MaxLength(100)]
        public string? ParentId {{ get; set; }}

        [StringLength(450)]
        public string Name {{ get; set; }} = string.Empty;
         
        [StringLength(450)]
        public string? ImageUrl {{ get; set; }}
         
        [StringLength(4000)]
        public string? Description {{ get; set; }}

        public decimal Price {{ get; set; }}

        public ProductType ProductType {{ get; set; }}
        
        public ProductStatus ProductStatus {{ get; set; }}

        public DateTime CreatedAt {{ get; set; }} = DateTime.UtcNow;

        public string CreatedBy {{ get; set; }} = string.Empty;
    }}
}}";
        }

        private string GenerateServerSideListingDataServiceContent(ProjectConfiguration config)
        {
            return $@"using Microsoft.EntityFrameworkCore;
using {config.ProjectName}.Framework;

namespace {config.ProjectName}.Server.Data.Services
{{
    public abstract class ServerSideListingDataService<TListingModel, TFilterBusinessObject> : IListingDataService<TListingModel, TFilterBusinessObject>
       where TFilterBusinessObject : BaseFilterBusinessObject
    {{
        public async Task<PagedDataList<TListingModel>> GetPaginatedItems(TFilterBusinessObject filterBusinessObject)
        {{
            return await MaterializeQueryAsync(filterBusinessObject, GetQuery(filterBusinessObject), GetTotalRows());
        }}

        public async Task<PagedDataList<TListingModel>> MaterializeQueryAsync(TFilterBusinessObject filterBusinessObject, IQueryable<TListingModel> query, int totalRows)
        {{
            var resultBusinessObject = new PagedDataList<TListingModel>();
            if (query != null)
            {{
                if (filterBusinessObject.UsePagination)
                {{
                    resultBusinessObject.TotalRows = totalRows;

                    if (resultBusinessObject.TotalRows == -1)
                        resultBusinessObject.TotalRows = query.Count();

                    resultBusinessObject.TotalPages = Convert.ToInt32(Math.Ceiling(resultBusinessObject.TotalRows / (double)filterBusinessObject.RowsPerPage));

                    query = query.Skip((filterBusinessObject.CurrentIndex - 1) * filterBusinessObject.RowsPerPage).Take(filterBusinessObject.RowsPerPage); 
                }}

                if (query is IAsyncEnumerable<TListingModel>)
                {{
                    resultBusinessObject.Items = await query.ToListAsync();
                }}
                else
                {{
                    resultBusinessObject.Items = query.ToList();
                }}
            }}
             
            return resultBusinessObject;
        }}

        public abstract IQueryable<TListingModel> GetQuery(TFilterBusinessObject filterBusinessObject);

        public virtual int GetTotalRows()
        {{
            return -1;
        }}
    }}
}}";
        }
    }
}
