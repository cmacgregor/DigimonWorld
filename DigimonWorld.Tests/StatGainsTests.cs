using Xunit;

public class StatGainsTests
{
    [Fact]
    public void ApplyStatGains_AddsEachFieldToTheMatchingStat()
    {
        var partner = new PartnerDigimon
        {
            Attack = 10,
            Defense = 10,
            Speed = 10,
            Brains = 10,
            MaxHP = 10,
            MaxMP = 10,
        };

        partner.ApplyStatGains(new StatGains { Attack = 5, MaxHP = 20 });

        Assert.Equal(15, partner.Attack);
        Assert.Equal(20, partner.MaxHP);
        // Untouched fields default to 0 - everything else is unaffected.
        Assert.Equal(10, partner.Defense);
        Assert.Equal(10, partner.Speed);
        Assert.Equal(10, partner.Brains);
        Assert.Equal(10, partner.MaxMP);
    }
}
