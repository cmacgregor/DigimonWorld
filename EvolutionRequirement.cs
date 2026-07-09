public class EvolutionRequirement
{
    public int? MinAttack { get; set; }
    public int? MinDefense { get; set; }
    public int? MinSpeed { get; set; }
    public int? MinBrains { get; set; }
    public int? MinMaxHP { get; set; }
    public int? MinMaxMP { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public int? MinWeight { get; set; }
    public int? MaxWeight { get; set; }
    public int? MinHappiness { get; set; }
    public int? MinDiscipline { get; set; }
    public int? MaxCareMistakes { get; set; }
    public int? MinBattlesFought { get; set; }
    public int? MinTechsLearned { get; set; }

    public bool IsMetBy(PartnerDigimon partner)
    {
        if (MinAttack.HasValue && partner.Attack < MinAttack.Value) return false;
        if (MinDefense.HasValue && partner.Defense < MinDefense.Value) return false;
        if (MinSpeed.HasValue && partner.Speed < MinSpeed.Value) return false;
        if (MinBrains.HasValue && partner.Brains < MinBrains.Value) return false;
        if (MinMaxHP.HasValue && partner.MaxHP < MinMaxHP.Value) return false;
        if (MinMaxMP.HasValue && partner.MaxMP < MinMaxMP.Value) return false;
        if (MinAge.HasValue && partner.Age < MinAge.Value) return false;
        if (MaxAge.HasValue && partner.Age > MaxAge.Value) return false;
        if (MinWeight.HasValue && partner.Weight < MinWeight.Value) return false;
        if (MaxWeight.HasValue && partner.Weight > MaxWeight.Value) return false;
        if (MinHappiness.HasValue && partner.Happiness < MinHappiness.Value) return false;
        if (MinDiscipline.HasValue && partner.Discipline < MinDiscipline.Value) return false;
        if (MaxCareMistakes.HasValue && partner.CareMistakes > MaxCareMistakes.Value) return false;
        if (MinBattlesFought.HasValue && partner.BattlesFought < MinBattlesFought.Value) return false;
        if (MinTechsLearned.HasValue && partner.LearnedTechs.Count < MinTechsLearned.Value) return false;

        return true;
    }
}
