using System.Collections.Generic;

// A queryable collection of every species' DigimonEvolutionData, keyed by
// SpeciesId - resolves "what can this Digimon evolve into" given just an
// id, instead of every caller needing to construct DigimonEvolutionData
// instances ad hoc.
public class DigimonEvolutionCatalog
{
    private readonly Dictionary<int, DigimonEvolutionData> _bySpeciesId = new();

    public void Add(DigimonEvolutionData speciesData)
    {
        _bySpeciesId[speciesData.SpeciesId] = speciesData;
    }

    public DigimonEvolutionData Get(int speciesId)
    {
        return _bySpeciesId.TryGetValue(speciesId, out var speciesData) ? speciesData : null;
    }
}
