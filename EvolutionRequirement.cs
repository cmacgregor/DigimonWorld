public class EvolutionRequirement
{
    public const int RequiredConditionCount = 3;

    public int TargetDigimonId { get; set; }
    public string TargetName { get; set; }

    public int? MinAttack { get; set; }
    public int? MinDefense { get; set; }
    public int? MinSpeed { get; set; }
    public int? MinBrains { get; set; }
    public int? MinMaxHP { get; set; }
    public int? MinMaxMP { get; set; }

    public int? MinWeight { get; set; }
    public int? MaxWeight { get; set; }
    public int? HappinessThreshold { get; set; }
    public int? DisciplineThreshold { get; set; }
    public int? MinCareMistakes { get; set; }
    public int? MaxCareMistakes { get; set; }
    public int? MinBattlesFought { get; set; }
    public int? MaxBattlesFought { get; set; }
    public int? MinTechsLearned { get; set; }
    public int? SpeciesBonusId { get; set; }

    public bool IsMetBy(PartnerDigimon partner)
    {
        if (partner.HoursInCurrentStage < partner.MinHoursInCurrentStage) return false;

        var satisfiedConditions = 0;

        if (IsStatCheckSatisfied(partner)) satisfiedConditions++;

        if ((MinWeight.HasValue || MaxWeight.HasValue)
            && (!MinWeight.HasValue || partner.Weight >= MinWeight.Value)
            && (!MaxWeight.HasValue || partner.Weight <= MaxWeight.Value))
        {
            satisfiedConditions++;
        }

        if (HappinessThreshold.HasValue && partner.Happiness >= HappinessThreshold.Value) satisfiedConditions++;
        if (DisciplineThreshold.HasValue && partner.Discipline >= DisciplineThreshold.Value) satisfiedConditions++;

        if ((MinCareMistakes.HasValue || MaxCareMistakes.HasValue)
            && (!MinCareMistakes.HasValue || partner.CareMistakes >= MinCareMistakes.Value)
            && (!MaxCareMistakes.HasValue || partner.CareMistakes <= MaxCareMistakes.Value))
        {
            satisfiedConditions++;
        }

        if ((MinBattlesFought.HasValue || MaxBattlesFought.HasValue)
            && (!MinBattlesFought.HasValue || partner.BattlesFought >= MinBattlesFought.Value)
            && (!MaxBattlesFought.HasValue || partner.BattlesFought <= MaxBattlesFought.Value))
        {
            satisfiedConditions++;
        }

        if (MinTechsLearned.HasValue && partner.LearnedTechs.Count >= MinTechsLearned.Value) satisfiedConditions++;

        if (SpeciesBonusId.HasValue && partner.SpeciesId == SpeciesBonusId.Value) satisfiedConditions++;

        return satisfiedConditions >= RequiredConditionCount;
    }

    // The stat check is one combined condition, not one per stat - it's
    // satisfied only if every threshold actually set here is met, whether
    // that's a single stat or all six.
    private bool IsStatCheckSatisfied(PartnerDigimon partner)
    {
        if (!MinAttack.HasValue && !MinDefense.HasValue && !MinSpeed.HasValue
            && !MinBrains.HasValue && !MinMaxHP.HasValue && !MinMaxMP.HasValue)
        {
            return false;
        }

        if (MinAttack.HasValue && partner.Attack < MinAttack.Value) return false;
        if (MinDefense.HasValue && partner.Defense < MinDefense.Value) return false;
        if (MinSpeed.HasValue && partner.Speed < MinSpeed.Value) return false;
        if (MinBrains.HasValue && partner.Brains < MinBrains.Value) return false;
        if (MinMaxHP.HasValue && partner.MaxHP < MinMaxHP.Value) return false;
        if (MinMaxMP.HasValue && partner.MaxMP < MinMaxMP.Value) return false;

        return true;
    }
}
