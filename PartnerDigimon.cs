using System.Collections.Generic;

public class PartnerDigimon : Digimon
{
    public const int LifespanGainOnEvolve = 96;

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

    public void Evolve(DigimonEvolutionData newSpeciesData)
    {
        Attack += DigimonEvolutionData.CalculateStatGain(Attack, newSpeciesData.ReferenceAttack);
        Defense += DigimonEvolutionData.CalculateStatGain(Defense, newSpeciesData.ReferenceDefense);
        Speed += DigimonEvolutionData.CalculateStatGain(Speed, newSpeciesData.ReferenceSpeed);
        Brains += DigimonEvolutionData.CalculateStatGain(Brains, newSpeciesData.ReferenceBrains);
        MaxHP += DigimonEvolutionData.CalculateStatGain(MaxHP, newSpeciesData.ReferenceMaxHP);
        MaxMP += DigimonEvolutionData.CalculateStatGain(MaxMP, newSpeciesData.ReferenceMaxMP);

        Lifespan += LifespanGainOnEvolve;
        ApplySpeciesChange(newSpeciesData);
    }

    // Triggered externally when the poop gauge is full - halves every stat
    // instead of applying the normal reference-stat gain formula. Still
    // grants the normal lifespan bonus.
    public void EvolveToSukamon(DigimonEvolutionData sukamonData)
    {
        ApplyStatPenaltyPercent(50);
        Lifespan += LifespanGainOnEvolve;
        ApplySpeciesChange(sukamonData);
    }

    // Triggered externally when a rookie hits its time gate without meeting
    // any evolution's requirements - a 20% stat penalty instead of a gain.
    // Still grants the normal lifespan bonus.
    public void EvolveToNumemon(DigimonEvolutionData numemonData)
    {
        ApplyStatPenaltyPercent(20);
        Lifespan += LifespanGainOnEvolve;
        ApplySpeciesChange(numemonData);
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

    private void ApplyStatPenaltyPercent(int percentLost)
    {
        Attack = ApplyPercent(Attack, percentLost);
        Defense = ApplyPercent(Defense, percentLost);
        Speed = ApplyPercent(Speed, percentLost);
        Brains = ApplyPercent(Brains, percentLost);
        MaxHP = ApplyPercent(MaxHP, percentLost);
        MaxMP = ApplyPercent(MaxMP, percentLost);
    }

    private static int ApplyPercent(int currentStat, int percentLost) => currentStat * (100 - percentLost) / 100;

    // resetStageTimer is false for same-level special evolutions (Kunemon,
    // Bakemon, etc.) - those don't reset progress toward the next real
    // level, but care mistakes and battle count reset like any evolution.
    private void ApplySpeciesChange(DigimonEvolutionData newSpeciesData, bool resetStageTimer = true)
    {
        SpeciesId = newSpeciesData.SpeciesId;
        Level = newSpeciesData.Level;
        CareMistakes = 0;
        BattlesFought = 0;

        if (resetStageTimer)
        {
            HoursInCurrentStage = 0;
        }

        PossibleEvolutions.Clear();
        PossibleEvolutions.AddRange(newSpeciesData.PossibleEvolutions);
    }
}
