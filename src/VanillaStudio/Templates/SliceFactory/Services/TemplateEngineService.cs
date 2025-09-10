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

        // First, process conditional blocks
        result = ProcessConditionalBlocks(result, parameters);

        // Then, process simple placeholders
        foreach (var parameter in parameters)
        {
            var placeholder = $"__{parameter.Key}__";
            result = result.Replace(placeholder, parameter.Value?.ToString() ?? string.Empty);
        }

        return result;
    }

    private string ProcessConditionalBlocks(string content, Dictionary<string, object> parameters)
    {
        var processedContent = content;

        // Process {{#if (eq UIFramework "FluentUI")}}...{{/if}} blocks
        var ifPattern = @"\{\{#if\s+\(eq\s+(\w+)\s+""([^""]+)""\)\}\}(.*?)\{\{/if\}\}";
        var regex = new System.Text.RegularExpressions.Regex(ifPattern,
            System.Text.RegularExpressions.RegexOptions.Singleline |
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        processedContent = regex.Replace(processedContent, match =>
        {
            var parameterName = match.Groups[1].Value;
            var expectedValue = match.Groups[2].Value;
            var blockContent = match.Groups[3].Value;

            // Check if the parameter exists and matches the expected value
            if (parameters.TryGetValue(parameterName, out var actualValue))
            {
                var actualValueStr = actualValue?.ToString() ?? string.Empty;
                if (string.Equals(actualValueStr, expectedValue, StringComparison.OrdinalIgnoreCase))
                {
                    return blockContent; // Include the content
                }
            }

            return string.Empty; // Exclude the content
        });

        // Process {{#unless (eq UIFramework "Bootstrap")}}...{{/unless}} blocks
        var unlessPattern = @"\{\{#unless\s+\(eq\s+(\w+)\s+""([^""]+)""\)\}\}(.*?)\{\{/unless\}\}";
        var unlessRegex = new System.Text.RegularExpressions.Regex(unlessPattern,
            System.Text.RegularExpressions.RegexOptions.Singleline |
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        processedContent = unlessRegex.Replace(processedContent, match =>
        {
            var parameterName = match.Groups[1].Value;
            var expectedValue = match.Groups[2].Value;
            var blockContent = match.Groups[3].Value;

            // Check if the parameter exists and does NOT match the expected value
            if (parameters.TryGetValue(parameterName, out var actualValue))
            {
                var actualValueStr = actualValue?.ToString() ?? string.Empty;
                if (!string.Equals(actualValueStr, expectedValue, StringComparison.OrdinalIgnoreCase))
                {
                    return blockContent; // Include the content
                }
            }
            else
            {
                return blockContent; // Include if parameter doesn't exist
            }

            return string.Empty; // Exclude the content
        });

        return processedContent;
    }

    public Dictionary<string, object> CreateParameterDictionary(
        string componentPrefix,
        string moduleNamespace,
        string projectNamespace,
        string primaryKeyType,
        string? uiFramework = null)
    {
        var pluralizedPrefix = _pluralizationService.Pluralize(componentPrefix);

        var parameters = new Dictionary<string, object>
        {
            ["ComponentPrefix"] = componentPrefix,
            ["componentPrefix"] = componentPrefix.ToLowerInvariant(),
            ["ComponentPrefixPlural"] = pluralizedPrefix,
            ["componentPrefixPlural"] = pluralizedPrefix.ToLowerInvariant(),
            ["moduleNamespace"] = moduleNamespace,
            ["projectNamespace"] = projectNamespace,
            ["primaryKeyType"] = primaryKeyType
        };

        // Add UIFramework parameter if provided
        if (!string.IsNullOrEmpty(uiFramework))
        {
            parameters["UIFramework"] = uiFramework;
        }

        return parameters;
    }
}