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
    public void Advance_WithinTheSameHour_OnlyMovesMinuteOfHour()
    {
        var time = new WorldTime { Day = 1, HourOfDay = 10, MinuteOfHour = 10 };

        time.Advance(5);

        Assert.Equal(1, time.Day);
        Assert.Equal(10, time.HourOfDay);
        Assert.Equal(15, time.MinuteOfHour);
    }

    [Fact]
    public void Advance_PastTheHour_RollsOverIntoHourOfDay()
    {
        var time = new WorldTime { Day = 1, HourOfDay = 10, MinuteOfHour = 55 };

        time.Advance(10);

        Assert.Equal(1, time.Day);
        Assert.Equal(11, time.HourOfDay);
        Assert.Equal(5, time.MinuteOfHour);
    }

    [Fact]
    public void Advance_PastMidnight_RollsOverIntoTheNextDay()
    {
        var time = new WorldTime { Day = 1, HourOfDay = 22 };

        time.Advance(5 * WorldTime.MinutesPerHour);

        Assert.Equal(2, time.Day);
        Assert.Equal(3, time.HourOfDay);
        Assert.Equal(0, time.MinuteOfHour);
    }

    [Fact]
    public void Advance_ByMultipleDays_AccumulatesCorrectly()
    {
        var time = new WorldTime { Day = 1, HourOfDay = 0 };

        time.Advance(50 * WorldTime.MinutesPerHour); // 2 full days (48h) + 2h

        Assert.Equal(3, time.Day);
        Assert.Equal(2, time.HourOfDay);
        Assert.Equal(0, time.MinuteOfHour);
    }

    [Fact]
    public void MinutesUntilWakeUp_BeforeWakeUpHour_ReturnsMinutesLaterTheSameDay()
    {
        var time = new WorldTime { HourOfDay = 3, MinuteOfHour = 0 };

        Assert.Equal(4 * WorldTime.MinutesPerHour, time.MinutesUntilWakeUp()); // 3:00 -> 7:00
    }

    [Fact]
    public void MinutesUntilWakeUp_AfterWakeUpHour_WrapsToTheNextDay()
    {
        var time = new WorldTime { HourOfDay = 22, MinuteOfHour = 30 };

        Assert.Equal(8 * WorldTime.MinutesPerHour + 30, time.MinutesUntilWakeUp()); // 22:30 -> 7:00 next day
    }

    [Fact]
    public void MinutesUntilWakeUp_ExactlyAtWakeUpHour_WrapsToTheNextDay()
    {
        var time = new WorldTime { HourOfDay = WorldTime.WakeUpHour, MinuteOfHour = 0 };

        Assert.Equal(24 * WorldTime.MinutesPerHour, time.MinutesUntilWakeUp());
    }
}
