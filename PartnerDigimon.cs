using System.Collections.Generic;

public class PartnerDigimon : Digimon
{
    public const int LifespanGainOnEvolve = 96;
    public const int TirednessGaugeMax = 100;
    public const int TiredThreshold = 80;
    public const int OverworkedThreshold = 100;

    public ActiveTimeEnum ActiveTime { get; set; }
    public string Nickname { get; set; }
    public int Happiness { get; set; }
    public int Discipline { get; set; }
    public int Virus { get; set; }
    public int Lives { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
    public bool Hungry { get; set; }
    public bool NeedsToPotty { get; set; }
    public int PottyGauge { get; set; }
    public bool Injured { get; set; }
    public bool Sleepy { get; set; }
    public int TirednessGauge { get; set; }
    public bool Sick { get; set; }
    public int CareMistakes { get; set; }
    public int Lifespan { get; set; }
    public int BattlesFought { get; set; }
    public int BattlesWon { get; set; }
    public int HoursInCurrentStage { get; set; }
    public int MinHoursInCurrentStage { get; set; }
    public LocationEnum CurrentLocation { get; set; }

    public List<EvolutionRequirement> PossibleEvolutions { get; } = new();

    public TirednessEnum Tiredness
    {
        get
        {
            if (TirednessGauge >= OverworkedThreshold) return TirednessEnum.Overworked;
            if (TirednessGauge >= TiredThreshold) return TirednessEnum.Tired;
            return TirednessEnum.Rested;
        }
    }

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

    // isSameLevelEvolution is true for special same-level cases (see
    // SpecialEvolutions) that still use the normal stat-gain formula but
    // shouldn't reset progress toward the next real level.
    public void Evolve(DigimonEvolutionData newSpeciesData, bool isSameLevelEvolution = false)
    {
        Attack += DigimonEvolutionData.CalculateStatGain(Attack, newSpeciesData.ReferenceAttack);
        Defense += DigimonEvolutionData.CalculateStatGain(Defense, newSpeciesData.ReferenceDefense);
        Speed += DigimonEvolutionData.CalculateStatGain(Speed, newSpeciesData.ReferenceSpeed);
        Brains += DigimonEvolutionData.CalculateStatGain(Brains, newSpeciesData.ReferenceBrains);
        MaxHP += DigimonEvolutionData.CalculateStatGain(MaxHP, newSpeciesData.ReferenceMaxHP);
        MaxMP += DigimonEvolutionData.CalculateStatGain(MaxMP, newSpeciesData.ReferenceMaxMP);

        Lifespan += LifespanGainOnEvolve;
        ApplySpeciesChange(newSpeciesData, resetStageTimer: !isSameLevelEvolution);
    }

    // Item-triggered evolution to a specific target. Still restricted to
    // this Mon's own PossibleEvolutions subset - an item can't send it
    // anywhere outside that. The only path with no stat or lifespan gain.
    public bool EvolveWithItem(DigimonEvolutionData targetSpeciesData)
    {
        var isValidTarget = false;
        foreach (var requirement in PossibleEvolutions)
        {
            if (requirement.TargetDigimonId == targetSpeciesData.SpeciesId)
            {
                isValidTarget = true;
                break;
            }
        }

        if (!isValidTarget)
        {
            return false;
        }

        ApplySpeciesChange(targetSpeciesData);
        return true;
    }

    // resetStageTimer is false for same-level special evolutions (see
    // SpecialEvolutions) - those don't reset progress toward the next
    // real level, but care mistakes and battle count reset like any
    // evolution. Internal so SpecialEvolutions can share it.
    internal void ApplySpeciesChange(DigimonEvolutionData newSpeciesData, bool resetStageTimer = true)
    {
        SpeciesId = newSpeciesData.SpeciesId;
        Level = newSpeciesData.Level;
        CareMistakes = 0;
        BattlesFought = 0;
        BattlesWon = 0;

        if (resetStageTimer)
        {
            HoursInCurrentStage = 0;
        }

        PossibleEvolutions.Clear();
        PossibleEvolutions.AddRange(newSpeciesData.PossibleEvolutions);
    }
}
