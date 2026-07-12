using Xunit;

public class DigimonEvolutionCatalogTests
{
    [Fact]
    public void Get_ReturnsTheSpeciesDataAddedForThatId()
    {
        var catalog = new DigimonEvolutionCatalog();
        var agumonData = new DigimonEvolutionData { SpeciesId = 1, Level = DigimonLevelEnum.Rookie };

        catalog.Add(agumonData);

        Assert.Same(agumonData, catalog.Get(1));
    }

    [Fact]
    public void Get_ReturnsNullForAnUnknownSpeciesId()
    {
        var catalog = new DigimonEvolutionCatalog();

        Assert.Null(catalog.Get(999));
    }

    [Fact]
    public void Add_WithSameSpeciesIdTwice_OverwritesThePreviousEntry()
    {
        var catalog = new DigimonEvolutionCatalog();
        var original = new DigimonEvolutionData { SpeciesId = 1, Level = DigimonLevelEnum.Baby };
        var replacement = new DigimonEvolutionData { SpeciesId = 1, Level = DigimonLevelEnum.InTraining };

        catalog.Add(original);
        catalog.Add(replacement);

        Assert.Same(replacement, catalog.Get(1));
    }

    [Fact]
    public void Get_DistinguishesBetweenDifferentSpeciesIds()
    {
        var catalog = new DigimonEvolutionCatalog();
        var agumonData = new DigimonEvolutionData { SpeciesId = 1 };
        var gabumonData = new DigimonEvolutionData { SpeciesId = 2 };

        catalog.Add(agumonData);
        catalog.Add(gabumonData);

        Assert.Same(agumonData, catalog.Get(1));
        Assert.Same(gabumonData, catalog.Get(2));
    }
}
