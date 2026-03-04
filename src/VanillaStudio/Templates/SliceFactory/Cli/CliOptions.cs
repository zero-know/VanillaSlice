namespace {{RootNamespace}}.SliceFactory.Cli;

/// <summary>
/// Command-line options for SliceFactory
/// </summary>
public class CliOptions
{
    public CliCommand Command { get; set; } = CliCommand.None;

    // Generate command options
    public string? ComponentPrefix { get; set; }
    public string? FeaturePluralName { get; set; }
    public string? Namespace { get; set; }
    public string? DirectoryName { get; set; }
    public string PrimaryKeyType { get; set; } = "Guid";
    public bool GenerateForm { get; set; } = false;
    public bool GenerateListing { get; set; } = false;
    public bool GenerateSelectList { get; set; } = false;
    public string SelectListModelType { get; set; } = "SelectOption";
    public string SelectListDataType { get; set; } = "string";
    public bool Preview { get; set; } = false;

    // Regenerate command options
    public string? SliceId { get; set; }

    // Help flag
    public bool ShowHelp { get; set; } = false;

    /// <summary>
    /// Parse command-line arguments into CliOptions
    /// </summary>
    public static CliOptions Parse(string[] args)
    {
        var options = new CliOptions();

        if (args.Length == 0)
        {
            return options; // No CLI args, run web UI
        }

        var i = 0;
        while (i < args.Length)
        {
            var arg = args[i].ToLowerInvariant();

            switch (arg)
            {
                // Commands
                case "generate":
                case "gen":
                case "g":
                    options.Command = CliCommand.Generate;
                    break;

                case "regenerate":
                case "regen":
                case "r":
                    options.Command = CliCommand.Regenerate;
                    break;

                case "regenerate-all":
                case "regen-all":
                case "ra":
                    options.Command = CliCommand.RegenerateAll;
                    break;

                case "list":
                case "ls":
                case "l":
                    options.Command = CliCommand.List;
                    break;

                case "remove":
                case "rm":
                case "delete":
                    options.Command = CliCommand.Remove;
                    break;

                // Options
                case "--prefix":
                case "-p":
                    options.ComponentPrefix = GetNextArg(args, ref i);
                    break;

                case "--plural":
                    options.FeaturePluralName = GetNextArg(args, ref i);
                    break;

                case "--namespace":
                case "-n":
                    options.Namespace = GetNextArg(args, ref i);
                    break;

                case "--directory":
                case "-d":
                    options.DirectoryName = GetNextArg(args, ref i);
                    break;

                case "--pk":
                case "--primary-key":
                    options.PrimaryKeyType = GetNextArg(args, ref i) ?? "Guid";
                    break;

                case "--form":
                case "-f":
                    options.GenerateForm = true;
                    break;

                case "--listing":
                case "-l":
                    options.GenerateListing = true;
                    break;

                case "--select-list":
                case "-s":
                    options.GenerateSelectList = true;
                    break;

                case "--select-model":
                    options.SelectListModelType = GetNextArg(args, ref i) ?? "SelectOption";
                    break;

                case "--select-type":
                    options.SelectListDataType = GetNextArg(args, ref i) ?? "string";
                    break;

                case "--preview":
                    options.Preview = true;
                    break;

                case "--id":
                    options.SliceId = GetNextArg(args, ref i);
                    break;

                case "--help":
                case "-h":
                case "help":
                case "?":
                    options.ShowHelp = true;
                    break;

                default:
                    // If it's a command argument without prefix, treat as slice ID for regenerate
                    if (options.Command == CliCommand.Regenerate && !arg.StartsWith("-") && string.IsNullOrEmpty(options.SliceId))
                    {
                        options.SliceId = args[i];
                    }
                    break;
            }

            i++;
        }

        // Auto-populate derived fields
        if (!string.IsNullOrEmpty(options.ComponentPrefix))
        {
            options.FeaturePluralName ??= options.ComponentPrefix + "s";
            options.DirectoryName ??= options.ComponentPrefix + "s";
            options.Namespace ??= options.ComponentPrefix + "s";
        }

        return options;
    }

    private static string? GetNextArg(string[] args, ref int i)
    {
        if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
        {
            i++;
            return args[i];
        }
        return null;
    }

    /// <summary>
    /// Validates the options for the generate command
    /// </summary>
    public (bool IsValid, string? ErrorMessage) ValidateForGenerate()
    {
        if (string.IsNullOrEmpty(ComponentPrefix))
            return (false, "Component prefix is required. Use --prefix <name>");

        if (string.IsNullOrEmpty(Namespace))
            return (false, "Namespace is required. Use --namespace <name>");

        if (!GenerateForm && !GenerateListing && !GenerateSelectList)
            return (false, "At least one slice type must be specified: --form, --listing, or --select-list");

        // Validate PrimaryKeyType
        var validPkTypes = new[] { "string", "Guid", "int", "long" };
        if (!validPkTypes.Contains(PrimaryKeyType, StringComparer.OrdinalIgnoreCase))
            return (false, $"Invalid primary key type. Must be one of: {string.Join(", ", validPkTypes)}");

        return (true, null);
    }

    /// <summary>
    /// Get help text for CLI usage
    /// </summary>
    public static string GetHelpText()
    {
        return """
            SliceFactory CLI - Generate vertical slice boilerplate code

            USAGE:
                dotnet run -- <command> [options]

            COMMANDS:
                generate, gen, g        Generate a new slice
                regenerate, regen, r    Regenerate a specific slice from manifest
                regenerate-all, ra      Regenerate all slices from manifest
                list, ls, l             List all slices in manifest
                remove, rm, delete      Remove a slice from manifest
                help                    Show this help message

            GENERATE OPTIONS:
                --prefix, -p <name>     Component prefix (e.g., Doctor) [required]
                --plural <name>         Plural name (default: <prefix>s)
                --namespace, -n <name>  Module namespace (default: <prefix>s)
                --directory, -d <name>  Directory name (default: <prefix>s)
                --pk <type>             Primary key type: Guid, string, int, long (default: Guid)
                --form, -f              Generate form slice
                --listing, -l           Generate listing slice
                --select-list, -s       Generate select list slice
                --select-model <type>   SelectList model type: SelectOption, Custom
                --select-type <type>    SelectList data type (default: string)
                --preview               Preview files without generating

            REGENERATE OPTIONS:
                --id <slice-id>         Slice ID to regenerate (or pass as argument)

            EXAMPLES:
                # Generate form and listing for Doctor
                dotnet run -- generate --prefix Doctor --namespace Doctors --form --listing

                # Generate with custom primary key
                dotnet run -- gen -p Patient -n Patients --pk string --form --listing

                # Preview without generating
                dotnet run -- generate --prefix Appointment --namespace Appointments --form --preview

                # Regenerate a specific slice
                dotnet run -- regenerate doctors-doctor

                # Regenerate all slices
                dotnet run -- regenerate-all

                # List all slices
                dotnet run -- list
            """;
    }
}

public enum CliCommand
{
    None,       // No command - run web UI
    Generate,
    Regenerate,
    RegenerateAll,
    List,
    Remove
}
