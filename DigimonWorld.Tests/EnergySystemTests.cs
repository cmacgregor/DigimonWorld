using Xunit;

public class EnergySystemTests
{
    [Fact]
    public void Advance_DoesNotDrainGauge_WhileNotHungry()
    {
        var energy = new EnergySystem { Gauge = 50 };

        energy.Advance(30, wholeHours: 0, hungry: false, isSleeping: false);

        Assert.Equal(50, energy.Gauge);
    }

    [Fact]
    public void Advance_DrainsGaugeByMinutes_WhileHungry_ClampedAtZero()
    {
        var energy = new EnergySystem { Gauge = 10 };

        energy.Advance(15, wholeHours: 0, hungry: true, isSleeping: false);

        Assert.Equal(0, energy.Gauge);
    }

    [Fact]
    public void Advance_ReturnsGramsLost_OncePerTenStarvingMinutes_AccumulatingAcrossCalls()
    {
        var energy = new EnergySystem { Gauge = 0 };

        var firstLoss = energy.Advance(7, wholeHours: 0, hungry: true, isSleeping: false);
        Assert.Equal(0, firstLoss);

        var secondLoss = energy.Advance(5, wholeHours: 0, hungry: true, isSleeping: false); // 12 accumulated -> 1 gram, 2 carried over
        Assert.Equal(1, secondLoss);
        Assert.Equal(2, energy.StarvationMinuteAccumulator);
    }

    [Fact]
    public void Advance_ReturnsNoLoss_WhileGaugeRemains()
    {
        var energy = new EnergySystem { Gauge = 100 };

        var gramsLost = energy.Advance(30, wholeHours: 0, hungry: true, isSleeping: false);

        Assert.Equal(0, gramsLost);
    }

    [Fact]
    public void Advance_ResetsStarvationAccumulator_OnceNoLongerStarving()
    {
        var energy = new EnergySystem { Gauge = 0, StarvationMinuteAccumulator = 5 };

        energy.Advance(10, wholeHours: 0, hungry: false, isSleeping: false);

        Assert.Equal(0, energy.StarvationMinuteAccumulator);
    }

    [Fact]
    public void Advance_WhileSleeping_ReturnsFlatPerHourLoss_WhenHungry()
    {
        var energy = new EnergySystem();

        var gramsLost = energy.Advance(9 * PartnerDigimon.MinutesPerHour, wholeHours: 9, hungry: true, isSleeping: true);

        Assert.Equal(9, gramsLost);
    }

    [Fact]
    public void Advance_WhileSleeping_ReturnsNoLoss_WhenNotHungry()
    {
        var energy = new EnergySystem();

        var gramsLost = energy.Advance(9 * PartnerDigimon.MinutesPerHour, wholeHours: 9, hungry: false, isSleeping: true);

        Assert.Equal(0, gramsLost);
    }
}
