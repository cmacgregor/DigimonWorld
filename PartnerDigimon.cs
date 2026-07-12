using System.Collections.Generic;

public class PartnerDigimon : Digimon
{
    public const int LifespanGainOnEvolve = 96;
    public const int BakemonChancePercent = 10;
    public const int DevimonChancePercent = 50;
    public const int AirdramonChancePercent = 30;
    public const int CoelamonChancePercent = 30;
    public const int CoelamonRequiredHoursInCurrentStage = 200;
    public const int NinjamonChancePercent = 30;
    public const int NinjamonRequiredBattlesWon = 50;
    public const int MonochromonChancePercent = 30;
    public const int MonochromonRequiredDefense = 500;
    public const int VademonPraiseOrScoldChancePercent = 50;
    public const int VademonPraiseOrScoldRequiredHoursInCurrentStage = 240;
    public const int VademonTimeoutRequiredHoursInCurrentStage = 360;
    public const int PhoenixmonChancePercent = 10;
    public const int SkullgreymonChancePercent = 10;
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

    // isSameLevelEvolution is true for special same-level cases (Bakemon,
    // Devimon, etc.) that still use the normal stat-gain formula but
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

    // Any Rookie, scolded while Happiness and Discipline are both 0.
    public bool CanEvolveToNanimon()
    {
        return Level == DigimonLevelEnum.Rookie && Happiness == 0 && Discipline == 0;
    }

    // No stat change at all, but still grants the normal lifespan bonus.
    // Same level as the Rookie it came from, so the stage timer doesn't
    // reset (only care mistakes/battle count do, like any evolution).
    public void EvolveToNanimon(DigimonEvolutionData nanimonData)
    {
        Lifespan += LifespanGainOnEvolve;
        ApplySpeciesChange(nanimonData, resetStageTimer: false);
    }

    // Any rookie except Penguinmon, when it loses a life. The life-loss
    // event and the percent roll both happen externally - this only
    // checks the deterministic part (right level, not the excluded
    // species). On success, call Evolve(bakemonData, isSameLevelEvolution: true).
    public bool CanEvolveToBakemon(int penguinmonSpeciesId)
    {
        return Level == DigimonLevelEnum.Rookie && SpeciesId != penguinmonSpeciesId;
    }

    // Angemon specifically, when it loses a life with Discipline <= 50.
    // Life-loss event and the 50% roll both stay external.
    public bool CanEvolveToDevimon(int angemonSpeciesId)
    {
        return SpeciesId == angemonSpeciesId && Discipline <= 50;
    }

    // Seadramon or Birdramon, wakes up with Discipline and Happiness both
    // at 100 and TirednessGauge at exactly 0 (not just Rested - the whole
    // range up to 79 counts as Rested, but this needs the literal floor).
    // Wake-up event and the 30% roll stay external.
    public bool CanEvolveToAirdramon(int seadramonSpeciesId, int birdramonSpeciesId)
    {
        return (SpeciesId == seadramonSpeciesId || SpeciesId == birdramonSpeciesId)
            && Discipline == 100
            && Happiness == 100
            && TirednessGauge == 0;
    }

    // Whamon or Shellmon, scolded or praised while the evolution counter
    // is at exactly 200h. Scold/praise event and the 30% roll stay
    // external.
    public bool CanEvolveToCoelamon(int whamonSpeciesId, int shellmonSpeciesId)
    {
        return (SpeciesId == whamonSpeciesId || SpeciesId == shellmonSpeciesId)
            && HoursInCurrentStage == CoelamonRequiredHoursInCurrentStage;
    }

    // Vegimon specifically, sleeps with Discipline 100 and more than 50
    // battles won. Sleep event and the 30% roll stay external.
    public bool CanEvolveToNinjamon(int vegimonSpeciesId)
    {
        return SpeciesId == vegimonSpeciesId
            && Discipline == 100
            && BattlesWon > NinjamonRequiredBattlesWon;
    }

    // Drimogemon specifically, sleeps with Discipline 100 and Defense of
    // at least 500. Sleep event and the 30% roll stay external.
    public bool CanEvolveToMonochromon(int drimogemonSpeciesId)
    {
        return SpeciesId == drimogemonSpeciesId
            && Discipline == 100
            && Defense >= MonochromonRequiredDefense;
    }

    // Any Champion, praised or scolded while the evolution counter is at
    // least 240h. Praise/scold event and the 50% roll stay external.
    public bool CanEvolveToVademonFromPraiseOrScold()
    {
        return Level == DigimonLevelEnum.Champion
            && HoursInCurrentStage >= VademonPraiseOrScoldRequiredHoursInCurrentStage;
    }

    // Any Champion that reaches a 360h evolution counter without meeting
    // any evolution to another Ultimate - deterministic, no roll needed
    // (same failure-fallback shape as Numemon, just at Champion level).
    public bool CanEvolveToVademonFromTimeout()
    {
        return Level == DigimonLevelEnum.Champion
            && HoursInCurrentStage >= VademonTimeoutRequiredHoursInCurrentStage
            && GetEligibleEvolution() == null;
    }

    // Kokatorimon specifically, when it loses a life. Life-loss event and
    // the 10% roll stay external.
    public bool CanEvolveToPhoenixmon(int kokatorimonSpeciesId)
    {
        return SpeciesId == kokatorimonSpeciesId;
    }

    // MetalGreymon or Megadramon, when it loses a life. Life-loss event
    // and the 10% roll stay external.
    public bool CanEvolveToSkullgreymon(int metalGreymonSpeciesId, int megadramonSpeciesId)
    {
        return SpeciesId == metalGreymonSpeciesId || SpeciesId == megadramonSpeciesId;
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
        BattlesWon = 0;

        if (resetStageTimer)
        {
            HoursInCurrentStage = 0;
        }

        PossibleEvolutions.Clear();
        PossibleEvolutions.AddRange(newSpeciesData.PossibleEvolutions);
    }
}
