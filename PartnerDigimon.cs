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
    public bool Injured { get; set; }
    public bool Sick { get; set; }
    public int CareMistakes { get; set; }
    public int Lifespan { get; set; }
    public int BattlesFought { get; set; }
    public int BattlesWon { get; set; }
    public int HoursInCurrentStage { get; set; }
    public int MinuteOfHour { get; set; }
    public int MinHoursInCurrentStage { get; set; }
    public LocationEnum CurrentLocation { get; set; }
    public bool IsTraining { get; set; }

    public HungerSystem Hunger { get; } = new();
    public EnergySystem Energy { get; } = new();
    public TirednessSystem Tiredness { get; } = new();
    public SleepSystem Sleep { get; } = new();
    public PottySystem Potty { get; } = new();

    public List<EvolutionRequirement> PossibleEvolutions { get; } = new();

    // Called by the world clock as it advances, in minutes. IsTraining
    // doubles Hunger's neglect care-mistake penalty, per the rule that
    // neglecting a hungry Digimon while it's training is worse - it's
    // state on the partner itself (set by whatever starts/stops a
    // training session), not something the world clock knows about.
    public void AdvanceTime(int minutes)
    {
        Tick(minutes, isSleeping: false);
    }

    // Invoked by the player's sleep command (enabled once Sleep.Sleepy is
    // true) rather than the regular world-clock tick - the caller passes
    // the whole sleep session's duration (e.g. ~9h) in one call. Freezes
    // Hunger's neglect timer and switches Energy's starvation weight loss
    // to the flat per-hour sleep rate, since Energy can't replenish
    // without eating.
    public void AdvanceTimeAsleep(int minutes)
    {
        Tick(minutes, isSleeping: true);
    }

    // HoursInCurrentStage only ticks once full 60-minute chunks
    // accumulate, so its existing whole-hour semantics (and the
    // hour-based evolution thresholds in EvolutionRequirement/
    // SpecialEvolutions) stay unchanged.
    private void Tick(int minutes, bool isSleeping)
    {
        MinuteOfHour += minutes;
        var wholeHours = MinuteOfHour / WorldTime.MinutesPerHour;
        MinuteOfHour %= WorldTime.MinutesPerHour;

        HoursInCurrentStage += wholeHours;
        Sleep.Advance(wholeHours);

        CareMistakes += Hunger.Advance(minutes, IsTraining, isSleeping);
        Weight -= Energy.Advance(minutes, wholeHours, Hunger.Hungry, isSleeping);
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
