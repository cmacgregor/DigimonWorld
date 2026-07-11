using System.Collections.Generic;

public class PartnerDigimon : Digimon
{
    public string Nickname { get; set; }
    public int Happiness { get; set; }
    public int Discipline { get; set; }
    public int Virus { get; set; }
    public int Lives { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
    public bool Hungry { get; set; }
    public bool Potty { get; set; }
    public bool Injured { get; set; }
    public bool Sleepy { get; set; }
    public bool Overworked { get; set; }
    public bool Sick { get; set; }
    public int CareMistakes { get; set; }
    public int Lifespan { get; set; }
    public int BattlesFought { get; set; }
    public int HoursInCurrentStage { get; set; }
    public int MinHoursInCurrentStage { get; set; }

    public List<EvolutionRequirement> PossibleEvolutions { get; } = new();

    public EvolutionRequirement GetEligibleEvolution()
    {
        foreach (var requirement in PossibleEvolutions)
        {
            if (requirement.IsMetBy(this))
            {
                return requirement;
            }
        }

        return null;
    }
}
