using System.Text.Json;
using System.Text.Json.Serialization;
using {{RootNamespace}}.SliceFactory.Models;

namespace {{RootNamespace}}.SliceFactory.Data;

/// <summary>
/// Persists slice metadata as a human-readable JSON file in DataFiles/slices-metadata.json.
/// Registered as a singleton so the list is loaded once and kept in memory.
/// </summary>
public class JsonFeatureStore
{
    private readonly string _filePath;
    private readonly SliceMetadata _data;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public JsonFeatureStore()
    {
        if (!Directory.Exists("DataFiles"))
            Directory.CreateDirectory("DataFiles");

        _filePath = "DataFiles/slices-metadata.json";
        _data = Load();
    }

    public List<Feature> Features => _data.Features;

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(_data, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, json, System.Text.Encoding.UTF8);
    }

    private SliceMetadata Load()
    {
        if (!File.Exists(_filePath))
            return new SliceMetadata();

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<SliceMetadata>(json, _jsonOptions) ?? new SliceMetadata();
        }
        catch
        {
            return new SliceMetadata();
        }
    }
}

public class SliceMetadata
{
    public List<Feature> Features { get; set; } = new();
}
