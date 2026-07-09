using System;

public class Tech
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DigimonTypeEnum Type { get; set; }
    public int MpCost { get; set; }
    public int Power { get; set; }
    public StatusEffectEnum StatusEffect { get; set; }
    public int StatusEffectChancePercent { get; set; }
}
