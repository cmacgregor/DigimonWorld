using Xunit;

public class WorldTimeTests
{
    [Theory]
    [InlineData(0, ActiveTimeEnum.Night)]
    [InlineData(5, ActiveTimeEnum.Night)]
    [InlineData(6, ActiveTimeEnum.Morning)]
    [InlineData(11, ActiveTimeEnum.Morning)]
    [InlineData(12, ActiveTimeEnum.Day)]
    [InlineData(17, ActiveTimeEnum.Day)]
    [InlineData(18, ActiveTimeEnum.Dusk)]
    [InlineData(23, ActiveTimeEnum.Dusk)]
    public void ActiveTime_ReflectsHourOfDayBand(int hourOfDay, ActiveTimeEnum expected)
    {
        var time = new WorldTime { HourOfDay = hourOfDay };

        Assert.Equal(expected, time.ActiveTime);
    }

    [Fact]
    public void Advance_WithinTheSameDay_OnlyMovesHourOfDay()
    {
        var time = new WorldTime { Day = 1, HourOfDay = 10 };

        time.Advance(5);

        Assert.Equal(1, time.Day);
        Assert.Equal(15, time.HourOfDay);
    }

    [Fact]
    public void Advance_PastMidnight_RollsOverIntoTheNextDay()
    {
        var time = new WorldTime { Day = 1, HourOfDay = 22 };

        time.Advance(5);

        Assert.Equal(2, time.Day);
        Assert.Equal(3, time.HourOfDay);
    }

    [Fact]
    public void Advance_ByMultipleDays_AccumulatesCorrectly()
    {
        var time = new WorldTime { Day = 1, HourOfDay = 0 };

        time.Advance(50); // 2 full days (48h) + 2h

        Assert.Equal(3, time.Day);
        Assert.Equal(2, time.HourOfDay);
    }
}
