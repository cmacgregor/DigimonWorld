using System.Collections.Generic;

public class Digimon : IBattleStats
{
    public int SpeciesId { get; set; }
    public string Name { get; set; }
    public string ModelName { get; set; }
    public string AnimationSet { get; set; }

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

    private readonly List<Tech> _learnedTechs = new();
    public IReadOnlyList<Tech> LearnedTechs => _learnedTechs;
    public void LearnTech(Tech tech) => _learnedTechs.Add(tech);

    public TechList ActiveTechs { get; } = new();

    public List<Evolution> PossibleEvolutions { get; } = new();
}
