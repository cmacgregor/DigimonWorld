using System.Collections.Generic;

public class DigimonEvolutionData
{
    public int SpeciesId { get; set; }
    public List<EvolutionRequirement> PossibleEvolutions { get; set; } = new();
}
