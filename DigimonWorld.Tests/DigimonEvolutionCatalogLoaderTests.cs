using System.IO;
using Xunit;

public class DigimonEvolutionCatalogLoaderTests
{
    [Fact]
    public void LoadFromDirectory_DeserializesEachJsonFileIntoTheCatalog()
    {
        var directory = Directory.CreateTempSubdirectory().FullName;
        try
        {
            File.WriteAllText(Path.Combine(directory, "Agumon.json"), """
                {
                    "SpeciesId": 1,
                    "Level": "Rookie",
                    "ReferenceAttack": 100
                }
                """);
            File.WriteAllText(Path.Combine(directory, "Gabumon.json"), """
                {
                    "SpeciesId": 2,
                    "Level": "Rookie",
                    "ReferenceAttack": 90
                }
                """);

            var catalog = DigimonEvolutionCatalogLoader.LoadFromDirectory(directory);

            var agumon = catalog.Get(1);
            Assert.NotNull(agumon);
            Assert.Equal(DigimonLevelEnum.Rookie, agumon.Level);
            Assert.Equal(100, agumon.ReferenceAttack);

            var gabumon = catalog.Get(2);
            Assert.NotNull(gabumon);
            Assert.Equal(90, gabumon.ReferenceAttack);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void LoadFromDirectory_DeserializesNestedPossibleEvolutions()
    {
        var directory = Directory.CreateTempSubdirectory().FullName;
        try
        {
            File.WriteAllText(Path.Combine(directory, "Agumon.json"), """
                {
                    "SpeciesId": 1,
                    "Level": "Rookie",
                    "PossibleEvolutions": [
                        {
                            "TargetDigimonId": 4,
                            "TargetName": "Greymon",
                            "MinAttack": 100
                        }
                    ]
                }
                """);

            var catalog = DigimonEvolutionCatalogLoader.LoadFromDirectory(directory);

            var agumon = catalog.Get(1);
            Assert.Single(agumon.PossibleEvolutions);
            Assert.Equal(4, agumon.PossibleEvolutions[0].TargetDigimonId);
            Assert.Equal("Greymon", agumon.PossibleEvolutions[0].TargetName);
            Assert.Equal(100, agumon.PossibleEvolutions[0].MinAttack);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void LoadFromDirectory_WithNoJsonFiles_ReturnsAnEmptyCatalog()
    {
        var directory = Directory.CreateTempSubdirectory().FullName;
        try
        {
            var catalog = DigimonEvolutionCatalogLoader.LoadFromDirectory(directory);

            Assert.Null(catalog.Get(1));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
