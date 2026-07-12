// Digivolution outcomes outside the normal requirement-based flow -
// forced/alternate evolutions triggered by specific in-game events
// (sleeping, being scolded/praised, losing a life, timing out) rather
// than satisfying an EvolutionRequirement. Trigger detection and any
// probability roll stay external; each method here only answers "is the
// deterministic part of the condition met" or "apply the resulting state
// change."
public static class SpecialEvolutions
{
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
    public const int KunemonChancePercent = 50;

    // Triggered externally when the poop gauge is full - halves every
    // stat instead of applying the normal reference-stat gain formula.
    // Still grants the normal lifespan bonus.
    public static void EvolveToSukamon(PartnerDigimon partner, DigimonEvolutionData sukamonData)
    {
        ApplyStatPenaltyPercent(partner, 50);
        partner.Lifespan += PartnerDigimon.LifespanGainOnEvolve;
        partner.ApplySpeciesChange(sukamonData);
    }

    // Triggered externally when a rookie hits its time gate without
    // meeting any evolution's requirements - a 20% stat penalty instead
    // of a gain. Still grants the normal lifespan bonus.
    public static void EvolveToNumemon(PartnerDigimon partner, DigimonEvolutionData numemonData)
    {
        ApplyStatPenaltyPercent(partner, 20);
        partner.Lifespan += PartnerDigimon.LifespanGainOnEvolve;
        partner.ApplySpeciesChange(numemonData);
    }

    // Any Rookie, scolded while Happiness and Discipline are both 0.
    public static bool CanEvolveToNanimon(PartnerDigimon partner)
    {
        return partner.Level == DigimonLevelEnum.Rookie && partner.Happiness == 0 && partner.Discipline == 0;
    }

    // No stat change at all, but still grants the normal lifespan bonus.
    // Same level, so the stage timer doesn't reset (only care
    // mistakes/battle count do, like any evolution).
    public static void EvolveToNanimon(PartnerDigimon partner, DigimonEvolutionData nanimonData)
    {
        partner.Lifespan += PartnerDigimon.LifespanGainOnEvolve;
        partner.ApplySpeciesChange(nanimonData, resetStageTimer: false);
    }

    // Any In-Training, sleeps in a particular area. Real location/world
    // state isn't designed yet - LocationEnum.PlaceholderArea stands in
    // for "the area" until that exists. Sleep event and the 50% roll
    // stay external.
    public static bool CanEvolveToKunemon(PartnerDigimon partner)
    {
        return partner.Level == DigimonLevelEnum.InTraining && partner.CurrentLocation == LocationEnum.PlaceholderArea;
    }

    // Any rookie except Penguinmon, when it loses a life. The life-loss
    // event and the percent roll both happen externally - this only
    // checks the deterministic part (right level, not the excluded
    // species). On success, call PartnerDigimon.Evolve(bakemonData,
    // isSameLevelEvolution: true).
    public static bool CanEvolveToBakemon(PartnerDigimon partner, int penguinmonSpeciesId)
    {
        return partner.Level == DigimonLevelEnum.Rookie && partner.SpeciesId != penguinmonSpeciesId;
    }

    // Angemon specifically, when it loses a life with Discipline <= 50.
    // Life-loss event and the 50% roll both stay external.
    public static bool CanEvolveToDevimon(PartnerDigimon partner, int angemonSpeciesId)
    {
        return partner.SpeciesId == angemonSpeciesId && partner.Discipline <= 50;
    }

    // Seadramon or Birdramon, wakes up with Discipline and Happiness both
    // at 100 and Tiredness.Gauge at exactly 0 (not just Rested - the
    // whole range up to 79 counts as Rested, but this needs the literal
    // floor). Wake-up event and the 30% roll stay external.
    public static bool CanEvolveToAirdramon(PartnerDigimon partner, int seadramonSpeciesId, int birdramonSpeciesId)
    {
        return (partner.SpeciesId == seadramonSpeciesId || partner.SpeciesId == birdramonSpeciesId)
            && partner.Discipline == 100
            && partner.Happiness == 100
            && partner.Tiredness.Gauge == 0;
    }

    // Whamon or Shellmon, scolded or praised while the evolution counter
    // is at exactly 200h. Scold/praise event and the 30% roll stay
    // external.
    public static bool CanEvolveToCoelamon(PartnerDigimon partner, int whamonSpeciesId, int shellmonSpeciesId)
    {
        return (partner.SpeciesId == whamonSpeciesId || partner.SpeciesId == shellmonSpeciesId)
            && partner.HoursInCurrentStage == CoelamonRequiredHoursInCurrentStage;
    }

    // Vegimon specifically, sleeps with Discipline 100 and more than 50
    // battles won. Sleep event and the 30% roll stay external.
    public static bool CanEvolveToNinjamon(PartnerDigimon partner, int vegimonSpeciesId)
    {
        return partner.SpeciesId == vegimonSpeciesId
            && partner.Discipline == 100
            && partner.BattlesWon > NinjamonRequiredBattlesWon;
    }

    // Drimogemon specifically, sleeps with Discipline 100 and Defense of
    // at least 500. Sleep event and the 30% roll stay external.
    public static bool CanEvolveToMonochromon(PartnerDigimon partner, int drimogemonSpeciesId)
    {
        return partner.SpeciesId == drimogemonSpeciesId
            && partner.Discipline == 100
            && partner.Defense >= MonochromonRequiredDefense;
    }

    // Any Champion, praised or scolded while the evolution counter is at
    // least 240h. Praise/scold event and the 50% roll stay external.
    public static bool CanEvolveToVademonFromPraiseOrScold(PartnerDigimon partner)
    {
        return partner.Level == DigimonLevelEnum.Champion
            && partner.HoursInCurrentStage >= VademonPraiseOrScoldRequiredHoursInCurrentStage;
    }

    // Any Champion that reaches a 360h evolution counter without meeting
    // any evolution to another Ultimate - deterministic, no roll needed
    // (same failure-fallback shape as Numemon, just at Champion level).
    public static bool CanEvolveToVademonFromTimeout(PartnerDigimon partner)
    {
        return partner.Level == DigimonLevelEnum.Champion
            && partner.HoursInCurrentStage >= VademonTimeoutRequiredHoursInCurrentStage
            && partner.GetEligibleEvolution() == null;
    }

    // Kokatorimon specifically, when it loses a life. Life-loss event and
    // the 10% roll stay external.
    public static bool CanEvolveToPhoenixmon(PartnerDigimon partner, int kokatorimonSpeciesId)
    {
        return partner.SpeciesId == kokatorimonSpeciesId;
    }

    // MetalGreymon or Megadramon, when it loses a life. Life-loss event
    // and the 10% roll stay external.
    public static bool CanEvolveToSkullgreymon(PartnerDigimon partner, int metalGreymonSpeciesId, int megadramonSpeciesId)
    {
        return partner.SpeciesId == metalGreymonSpeciesId || partner.SpeciesId == megadramonSpeciesId;
    }

    private static void ApplyStatPenaltyPercent(PartnerDigimon partner, int percentLost)
    {
        partner.Attack = ApplyPercent(partner.Attack, percentLost);
        partner.Defense = ApplyPercent(partner.Defense, percentLost);
        partner.Speed = ApplyPercent(partner.Speed, percentLost);
        partner.Brains = ApplyPercent(partner.Brains, percentLost);
        partner.MaxHP = ApplyPercent(partner.MaxHP, percentLost);
        partner.MaxMP = ApplyPercent(partner.MaxMP, percentLost);
    }

    private static int ApplyPercent(int currentStat, int percentLost) => currentStat * (100 - percentLost) / 100;
}
