using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

// Reads one JSON file per species from a directory and populates a
// DigimonEvolutionCatalog. Loads everything eagerly at startup - at this
// data's scale (a few hundred species, a dozen-ish fields each) that's
// simpler than lazy-loading and costs nothing worth optimizing away yet.
public static class DigimonEvolutionCatalogLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public static DigimonEvolutionCatalog LoadFromDirectory(string directoryPath)
    {
        var catalog = new DigimonEvolutionCatalog();

        foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*.json"))
        {
            var json = File.ReadAllText(filePath);
            var speciesData = JsonSerializer.Deserialize<DigimonEvolutionData>(json, Options);
            catalog.Add(speciesData);
        }

        return catalog;
    }
}
