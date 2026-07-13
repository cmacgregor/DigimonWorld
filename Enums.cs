using System;

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

// Which context(s) an item can be used in - a flag set rather than a
// single exclusive category, since items overlap (e.g. something edible
// that's also usable in battle). KeyItem covers items that aren't
// consumed through Feed or Battle at all (story/quest triggers).
[Flags]
public enum ItemUsageEnum
{
    None = 0,
    Feed = 1,
    Battle = 2,
    Evolution = 4,
    KeyItem = 8,
}
