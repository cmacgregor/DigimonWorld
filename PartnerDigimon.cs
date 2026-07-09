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

    public Evolution GetEligibleEvolution()
    {
        foreach (var evolution in PossibleEvolutions)
        {
            if (evolution.Requirement.IsMetBy(this))
            {
                return evolution;
            }
        }

        return null;
    }

    public void Evolve(DigimonSpecies newSpecies)
    {
        SpeciesId = newSpecies.Id;
        Name = newSpecies.Name;
        ModelName = newSpecies.ModelName;
        AnimationSet = newSpecies.AnimationSet;
        Type = newSpecies.Type;
        ActiveTime = newSpecies.ActiveTime;
        Specialty1 = newSpecies.Specialty1;
        Specialty2 = newSpecies.Specialty2;
        FinishingMove = newSpecies.FinishingMove;

        MaxHP = ApplyStatGain(MaxHP, newSpecies.BaseMaxHP);
        MaxMP = ApplyStatGain(MaxMP, newSpecies.BaseMaxMP);
        Attack = ApplyStatGain(Attack, newSpecies.BaseAttack);
        Defense = ApplyStatGain(Defense, newSpecies.BaseDefense);
        Speed = ApplyStatGain(Speed, newSpecies.BaseSpeed);
        Brains = ApplyStatGain(Brains, newSpecies.BaseBrains);

        Lifespan += newSpecies.LifespanBonus;

        PossibleEvolutions.Clear();
        PossibleEvolutions.AddRange(newSpecies.PossibleEvolutions);
    }

    // Placeholder growth formula until the real one is designed.
    private static int ApplyStatGain(int currentStat, int speciesBaseStat) => currentStat + speciesBaseStat;
}
