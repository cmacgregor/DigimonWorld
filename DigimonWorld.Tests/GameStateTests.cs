using Xunit;

public class GameStateTests
{
    [Fact]
    public void AdvanceTime_AdvancesWorldClockAndActivePartnerTogether()
    {
        var gameState = new GameState
        {
            CurrentPartner = new PartnerDigimon { HungerGauge = 0 },
        };

        gameState.AdvanceTime(5);

        Assert.Equal(5, gameState.CurrentTime.HourOfDay);
        Assert.Equal(5, gameState.CurrentPartner.HungerGauge);
    }

    [Fact]
    public void AdvanceTime_WithNoCurrentPartner_StillAdvancesTheWorldClock()
    {
        var gameState = new GameState();

        gameState.AdvanceTime(5);

        Assert.Equal(5, gameState.CurrentTime.HourOfDay);
    }
}
