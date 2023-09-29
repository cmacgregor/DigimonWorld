
public class PartnerDigimon : Digimon, IRaisingStats
{
    
}

public class Digimon : IBattleStats
{
    public int Id { get; set; }
    public Name { get; set; }
    public string ModelName { get; set; }
    public string AnimationSet { get; set; } 

}

public interface IStats
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
    public int FinishingMove {get; set; }
    public List<Guid> TechList { get; set; }
    public Dictionary<string, int> Stats { get}
}

public interface IRaisingStats 
{
    public int Nickname { get; set; }
    public int Happiness { get; set; }
    public int Discipline { get; set; }
    public int Virus { get; set; }
    public int Lives { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
    public bool Hungry { get; set; }
    public bool Potty { get; set; }
    public bool Injured {get; set; }
    public bool Sleepy { get; set; }
    public bool Overworked { get; set; }
    public bool Sick { get; set; }
    public int CareMistakes { get; set; }
}