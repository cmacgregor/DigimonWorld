
public enum DigimonTypeEnum {
    None,
    Vaccine,
    Data, 
    Virus,
}

public enum SpecialtiesEnum {
    None,
    Fire, 
    Battle,
    Air,
    Earth, 
    Water, 
    Mecha,
    Filthy,
}

public enum ActiveTimeEnum
{
    None,
    Morning,
    Day,
    Dusk,
    Night,
}

public enum StatusEffectEnum
{
    None,
    Confuse,
    Flat,
    Poison,
    Stun,
}

public enum DigimonLevelEnum
{
    None,
    Baby,
    InTraining,
    Rookie,
    Champion,
    Ultimate,
}

public enum TirednessEnum
{
    Rested,
    Tired,
    Overworked,
}

// TODO: Placeholder until real location/world state is designed - just
// enough to represent "which area is the Digimon currently in." Replace
// PlaceholderArea with the real set of areas once that system exists.
public enum LocationEnum
{
    None,
    PlaceholderArea,
}
