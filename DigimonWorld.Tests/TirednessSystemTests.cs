using Xunit;

public class TirednessSystemTests
{
    [Theory]
    [InlineData(79, TirednessEnum.Rested)]
    [InlineData(80, TirednessEnum.Tired)]
    [InlineData(99, TirednessEnum.Tired)]
    [InlineData(100, TirednessEnum.Overworked)]
    public void State_ReflectsGaugeThresholds(int gauge, TirednessEnum expected)
    {
        var tiredness = new TirednessSystem { Gauge = gauge };

        Assert.Equal(expected, tiredness.State);
    }
}
