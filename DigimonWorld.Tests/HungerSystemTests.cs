using Xunit;

public class HungerSystemTests
{
    [Fact]
    public void Hungry_IsTrueOnceGaugeReachesZeroOrBelow()
    {
        var notHungry = new HungerSystem { Gauge = 1 };
        var justHungry = new HungerSystem { Gauge = 0 };
        var pastHungry = new HungerSystem { Gauge = -5 };

        Assert.False(notHungry.Hungry);
        Assert.True(justHungry.Hungry);
        Assert.True(pastHungry.Hungry);
    }

    [Fact]
    public void Advance_CountsDownTheGaugeByMinutes()
    {
        var hunger = new HungerSystem { Gauge = 200 };

        hunger.Advance(180, isTraining: false, isSleeping: false);

        Assert.Equal(20, hunger.Gauge);
    }

    [Fact]
    public void Advance_AppliesOneCareMistake_WhenNeglectCrossesOneAndAHalfHours()
    {
        var hunger = new HungerSystem { Gauge = 1 };

        // Crosses from Gauge=1 to -90 in one tick (91 minutes).
        var careMistakes = hunger.Advance(91, isTraining: false, isSleeping: false);

        Assert.True(hunger.CareMistakeApplied);
        Assert.Equal(1, careMistakes);
    }

    [Fact]
    public void Advance_ResetsGauge_WhenNeglectCareMistakeFires()
    {
        var hunger = new HungerSystem { Gauge = 1 };

        hunger.Advance(91, isTraining: false, isSleeping: false);

        Assert.Equal(HungerSystem.GaugeResetValueAfterNeglect, hunger.Gauge);
    }

    [Fact]
    public void Advance_AppliesTwoCareMistakes_WhenNeglectCrossesWhileTraining()
    {
        var hunger = new HungerSystem { Gauge = 1 };

        var careMistakes = hunger.Advance(91, isTraining: true, isSleeping: false);

        Assert.True(hunger.CareMistakeApplied);
        Assert.Equal(2, careMistakes);
    }

    [Fact]
    public void Advance_OnlyAppliesTheCareMistakeOnce()
    {
        var hunger = new HungerSystem { Gauge = 1 };

        hunger.Advance(91, isTraining: false, isSleeping: false); // crosses the threshold
        var secondCareMistakes = hunger.Advance(300, isTraining: false, isSleeping: false); // still neglected

        Assert.Equal(0, secondCareMistakes);
    }

    [Fact]
    public void Advance_DoesNotApplyACareMistake_WhenStillWithinTheNeglectWindow()
    {
        var hunger = new HungerSystem { Gauge = 1 };

        // Hungry (gauge <= 0) but not yet 90 minutes past it.
        var careMistakes = hunger.Advance(50, isTraining: false, isSleeping: false);

        Assert.False(hunger.CareMistakeApplied);
        Assert.Equal(0, careMistakes);
    }

    [Fact]
    public void Advance_WhileSleeping_FreezesTheNeglectTimer_AndDoesNotResetTheGauge()
    {
        var hunger = new HungerSystem { Gauge = 1 };

        var careMistakes = hunger.Advance(91, isTraining: false, isSleeping: true);

        Assert.False(hunger.CareMistakeApplied);
        Assert.Equal(0, careMistakes);
        Assert.Equal(-90, hunger.Gauge);
    }
}
