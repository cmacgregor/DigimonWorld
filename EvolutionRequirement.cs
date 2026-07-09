public class EvolutionRequirement
{
    public const int RequiredConditionCount = 3;

    public int MinHoursInCurrentStage { get; set; }

    public int? MinCoreStats { get; set; }
    public int? MinMaxHP { get; set; }
    public int? MinMaxMP { get; set; }
    public int? MinWeight { get; set; }
    public int? MaxWeight { get; set; }
    public int? MinHappiness { get; set; }
    public int? MinDiscipline { get; set; }
    public int? MaxCareMistakes { get; set; }
    public int? MinBattlesFought { get; set; }
    public int? MinTechsLearned { get; set; }
    public int? SpeciesBonusId { get; set; }

    public bool IsMetBy(PartnerDigimon partner)
    {
        // Being the bonus species is a free pass - it never blocks anyone else,
        // it just lets this one species skip every other condition below.
        if (SpeciesBonusId.HasValue && partner.SpeciesId == SpeciesBonusId.Value) return true;

        if (partner.HoursInCurrentStage < MinHoursInCurrentStage) return false;

        var satisfiedConditions = 0;

        if (MinCoreStats.HasValue
            && partner.Attack >= MinCoreStats.Value
            && partner.Defense >= MinCoreStats.Value
            && partner.Speed >= MinCoreStats.Value
            && partner.Brains >= MinCoreStats.Value)
        {
            satisfiedConditions++;
        }

        if (MinMaxHP.HasValue && partner.MaxHP >= MinMaxHP.Value) satisfiedConditions++;
        if (MinMaxMP.HasValue && partner.MaxMP >= MinMaxMP.Value) satisfiedConditions++;

        if ((MinWeight.HasValue || MaxWeight.HasValue)
            && (!MinWeight.HasValue || partner.Weight >= MinWeight.Value)
            && (!MaxWeight.HasValue || partner.Weight <= MaxWeight.Value))
        {
            satisfiedConditions++;
        }

        if (MinHappiness.HasValue && partner.Happiness >= MinHappiness.Value) satisfiedConditions++;
        if (MinDiscipline.HasValue && partner.Discipline >= MinDiscipline.Value) satisfiedConditions++;
        if (MaxCareMistakes.HasValue && partner.CareMistakes <= MaxCareMistakes.Value) satisfiedConditions++;
        if (MinBattlesFought.HasValue && partner.BattlesFought >= MinBattlesFought.Value) satisfiedConditions++;
        if (MinTechsLearned.HasValue && partner.LearnedTechs.Count >= MinTechsLearned.Value) satisfiedConditions++;

        return satisfiedConditions >= RequiredConditionCount;
    }
}
