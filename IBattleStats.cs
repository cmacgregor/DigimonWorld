using System.Collections.Generic;

public interface IBattleStats
{
    public DigimonTypeEnum Type { get; set; }
    public ActiveTimeEnum ActiveTime { get; set; }
    public SpecialtiesEnum Specialty1 { get; set; }
    public SpecialtiesEnum Specialty2 { get; set; }
    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }
    public int CurrentMP { get; set; }
    public int MaxMP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public int Brains { get; set; }
    public int FinishingMove { get; set; }

    public IReadOnlyList<Tech> LearnedTechs { get; }
    void LearnTech(Tech tech);
    public TechList ActiveTechs { get; }
}
