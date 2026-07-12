using Xunit;

public class GameStateTests
{
    [Fact]
    public void AdvanceTime_AdvancesWorldClockAndActivePartnerTogether()
    {
        var gameState = new GameState
        {
            CurrentPartner = new PartnerDigimon(),
        };

        gameState.AdvanceTime(5 * WorldTime.MinutesPerHour);

        Assert.Equal(5, gameState.CurrentTime.HourOfDay);
        Assert.Equal(5, gameState.CurrentPartner.HoursInCurrentStage);
    }

    [Fact]
    public void AdvanceTime_HonorsThePartnersOwnIsTrainingState()
    {
        var gameState = new GameState
        {
            CurrentPartner = new PartnerDigimon { Hunger = { Gauge = 1 }, IsTraining = true },
        };

        gameState.AdvanceTime(91);

        Assert.Equal(2, gameState.CurrentPartner.CareMistakes);
    }

    [Fact]
    public void AdvanceTime_WithNoCurrentPartner_StillAdvancesTheWorldClock()
    {
        var gameState = new GameState();

        gameState.AdvanceTime(5 * WorldTime.MinutesPerHour);

        Assert.Equal(5, gameState.CurrentTime.HourOfDay);
    }

    [Fact]
    public void Sleep_AdvancesWorldClockAndTellsThePartnerItsAsleep()
    {
        var gameState = new GameState
        {
            CurrentPartner = new PartnerDigimon { Hunger = { Gauge = 1 } },
        };

        gameState.Sleep(91); // would incur a neglect care mistake while awake

        Assert.Equal(1, gameState.CurrentTime.HourOfDay);
        Assert.Equal(0, gameState.CurrentPartner.CareMistakes);
    }
}
