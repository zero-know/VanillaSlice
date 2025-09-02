using VanillaSlice.Bootstrapper.Models;

namespace VanillaSlice.Bootstrapper.Services
{
    public class CommonProjectGenerator
    {
        public List<GeneratedFile> GenerateCommonProject(ProjectConfiguration config)
        {
            var files = new List<GeneratedFile>();

            // Project file
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Common/{config.ProjectName}.Common/{config.ProjectName}.Common.csproj",
                Content = GenerateCommonProjectFile(config),
                Type = FileType.ProjectFile
            });

            // Readme file
            files.Add(new GeneratedFile
            {
                RelativePath = $"{config.ProjectName}.Common/{config.ProjectName}.Common/readme.md",
                Content = GenerateReadmeContent(config),
                Type = FileType.Other
            });

            // Sample enums (only if sample data is enabled)
            if (config.IncludeSampleData)
            {
                files.Add(new GeneratedFile
                {
                    RelativePath = $"{config.ProjectName}.Common/{config.ProjectName}.Common/Enums/ProductStatus.cs",
                    Content = GenerateProductStatusContent(config),
                    Type = FileType.CSharpCode
                });

                files.Add(new GeneratedFile
                {
                    RelativePath = $"{config.ProjectName}.Common/{config.ProjectName}.Common/Enums/ProductType.cs",
                    Content = GenerateProductTypeContent(config),
                    Type = FileType.CSharpCode
                });
            }

            return files;
        }

        private string GenerateCommonProjectFile(ProjectConfiguration config)
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>";
        }

        private string GenerateReadmeContent(ProjectConfiguration config)
        {
            return $"This project contains the common code for the {config.ProjectName} solution. It is a class library that is referenced by all other projects. Most suitable candidates are Enums, which will be used across the solution including efcore entities. If we keep enums with entities, it cannot be referred to UI razor and client shared projects.";
        }

        private string GenerateProductStatusContent(ProjectConfiguration config)
        {
            return $@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {config.ProjectName}.Common.Enums
{{
    /// <summary>
    /// Byte type enum representing the status of a product for datatype optimization demonstration
    /// </summary>
    public enum ProductStatus : byte
    {{
        Inactive = 0,
        Active = 1,
        Discontinued = 2,
        OutOfStock = 3
    }}
}}";
        }

        private string GenerateProductTypeContent(ProjectConfiguration config)
        {
            return $@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {config.ProjectName}.Common.Enums
{{
    public enum ProductType
    {{
        Unknown = 0,
        Physical = 1,
        Digital = 2,
        Service = 4,
    }}
}}";
        }
    }
}
