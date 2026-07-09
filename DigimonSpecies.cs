using System.Collections.Generic;

public class DigimonSpecies
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ModelName { get; set; }
    public string AnimationSet { get; set; }

    public DigimonTypeEnum Type { get; set; }
    public ActiveTimeEnum ActiveTime { get; set; }
    public SpecialtiesEnum Specialty1 { get; set; }
    public SpecialtiesEnum Specialty2 { get; set; }

    public int BaseMaxHP { get; set; }
    public int BaseMaxMP { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }
    public int BaseSpeed { get; set; }
    public int BaseBrains { get; set; }
    public int FinishingMove { get; set; }
    public int LifespanBonus { get; set; }

    public List<Tech> LearnableTechs { get; set; } = new();
    public List<Evolution> PossibleEvolutions { get; set; } = new();
}
