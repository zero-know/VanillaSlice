using Microsoft.TemplateEngine.Edge;
using Microsoft.TemplateEngine.Edge.Settings;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects;
using System.Text;
using System.Text.RegularExpressions;

namespace {{RootNamespace}}.SliceFactory.Services;

public class TemplateEngineService
{
    private readonly IWebHostEnvironment _environment;
    private readonly PluralizationService _pluralizationService;

    public TemplateEngineService(IWebHostEnvironment environment, PluralizationService pluralizationService)
    {
        _environment = environment;
        _pluralizationService = pluralizationService;
    }

    public async Task<Dictionary<string, string>> ProcessTemplatesAsync(
        string projectType,
        string sliceType,
        Dictionary<string, object> parameters)
    {
        var templatePath = Path.Combine(_environment.ContentRootPath, "Templates", projectType, sliceType);

        if (!Directory.Exists(templatePath))
        {
            throw new DirectoryNotFoundException($"Template directory not found: {templatePath}");
        }

        var processedFiles = new Dictionary<string, string>();
        var templateFiles = Directory.GetFiles(templatePath, "*", SearchOption.AllDirectories);

        foreach (var templateFile in templateFiles)
        {
            var templateContent = await File.ReadAllTextAsync(templateFile, Encoding.UTF8);
            var processedContent = ProcessTemplate(templateContent, parameters);

            // Normalize line endings to CRLF for Windows compatibility
            processedContent = processedContent.Replace("\r\n", "\n").Replace("\n", "\r\n");

            var relativePath = Path.GetRelativePath(templatePath, templateFile);
            var processedFileName = ProcessTemplate(relativePath, parameters);

            processedFiles[processedFileName] = processedContent;
        }

        return processedFiles;
    }

    private string ProcessTemplate(string template, Dictionary<string, object> parameters)
    {
        var result = template;

        foreach (var parameter in parameters)
        {
            var placeholder = $"__{parameter.Key}__";
            result = result.Replace(placeholder, parameter.Value?.ToString() ?? string.Empty);
        }

        return result;
    }

    public Dictionary<string, object> CreateParameterDictionary(
        string componentPrefix,
        string moduleNamespace,
        string projectNamespace,
        string primaryKeyType)
    {
        var pluralizedPrefix = _pluralizationService.Pluralize(componentPrefix);

        return new Dictionary<string, object>
        {
            ["ComponentPrefix"] = componentPrefix,
            ["componentPrefix"] = componentPrefix.ToLowerInvariant(),
            ["ComponentPrefixPlural"] = pluralizedPrefix,
            ["componentPrefixPlural"] = pluralizedPrefix.ToLowerInvariant(),
            ["moduleNamespace"] = moduleNamespace,
            ["projectNamespace"] = projectNamespace,
            ["primaryKeyType"] = primaryKeyType
        };
    }
}