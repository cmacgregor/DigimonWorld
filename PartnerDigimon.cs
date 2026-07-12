using System.Collections.Generic;

public class PartnerDigimon : Digimon
{
    public const int LifespanGainOnEvolve = 96;
    public const int TirednessGaugeMax = 100;
    public const int TiredThreshold = 80;
    public const int OverworkedThreshold = 100;

    // HungerGauge ticks in half-hours (not whole hours) so the 1.5h
    // neglect window divides evenly, without switching every other timer
    // in the codebase (HoursInCurrentStage, SleepGauge, etc.) to
    // fractional hours just for this one rule.
    public const int HungerTicksPerHour = 2;
    public const int HungerGaugeMax = 16; // placeholder: ~8h before Hungry
    public const int HungerNeglectCareMistakeTicks = 3; // 1.5h past Hungry
    public const int HungerNeglectCareMistakesWhileTraining = 2;

    public ActiveTimeEnum ActiveTime { get; set; }
    public string Nickname { get; set; }
    public int Happiness { get; set; }
    public int Discipline { get; set; }
    public int Virus { get; set; }
    public int Lives { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
    public int HungerGauge { get; set; }
    // Set once the first time HungerGauge crosses the neglect threshold,
    // so the care mistake fires once per hungry episode, not every tick.
    // Feeding (not built yet) is expected to reset this back to false
    // along with HungerGauge.
    public bool HungerCareMistakeApplied { get; set; }
    public bool NeedsToPotty { get; set; }
    public int PottyGauge { get; set; }
    public bool Injured { get; set; }
    public bool Sleepy { get; set; }
    public int SleepGauge { get; set; }
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

    // Unlike Tiredness/PottyGauge, HungerGauge counts down - it's a timer
    // until hungry, not a meter that builds up to it.
    public bool Hungry => HungerGauge <= 0;

    // Called by the world clock as it advances. isTraining doubles the
    // care-mistake penalty below, per the rule that neglecting a hungry
    // Digimon while it's training is worse. SleepGauge's rate is still a
    // 1:1 placeholder (1 point per hour) until real pacing is designed.
    // Sleepy/NeedsToPotty aren't flipped here; those threshold decisions
    // stay external, same as PottyGauge's "full" condition already does.
    public void AdvanceTime(int hours, bool isTraining = false)
    {
        HoursInCurrentStage += hours;
        SleepGauge += hours;

        var previousHungerGauge = HungerGauge;
        HungerGauge -= hours * HungerTicksPerHour;

        var neglectThreshold = -HungerNeglectCareMistakeTicks;
        if (!HungerCareMistakeApplied && previousHungerGauge > neglectThreshold && HungerGauge <= neglectThreshold)
        {
            CareMistakes += isTraining ? HungerNeglectCareMistakesWhileTraining : 1;
            HungerCareMistakeApplied = true;
        }
    }

    // Shared application point for training/battle/food stat gains -
    // each source computes its own StatGains elsewhere and applies it
    // here, regardless of which stats it actually touches.
    public void ApplyStatGains(StatGains gains)
    {
        Attack += gains.Attack;
        Defense += gains.Defense;
        Speed += gains.Speed;
        Brains += gains.Brains;
        MaxHP += gains.MaxHP;
        MaxMP += gains.MaxMP;
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
