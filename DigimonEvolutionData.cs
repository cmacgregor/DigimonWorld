using System.Collections.Generic;

public class DigimonEvolutionData
{
    public int SpeciesId { get; set; }
    public List<EvolutionRequirement> PossibleEvolutions { get; set; } = new();

    public int ReferenceAttack { get; set; }
    public int ReferenceDefense { get; set; }
    public int ReferenceSpeed { get; set; }
    public int ReferenceBrains { get; set; }
    public int ReferenceMaxHP { get; set; }
    public int ReferenceMaxMP { get; set; }

    public static int CalculateStatGain(int currentStat, int referenceStat)
    {
        return currentStat <= referenceStat
            ? (referenceStat - currentStat) / 2
            : referenceStat / 10;
    }
}
