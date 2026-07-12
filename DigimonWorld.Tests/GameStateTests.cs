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

        gameState.AdvanceTime(5);

        Assert.Equal(5, gameState.CurrentTime.HourOfDay);
        Assert.Equal(5, gameState.CurrentPartner.HoursInCurrentStage);
    }

    [Fact]
    public void AdvanceTime_PassesIsTrainingThroughToThePartner()
    {
        var gameState = new GameState
        {
            CurrentPartner = new PartnerDigimon { HungerGauge = 1 },
        };

        gameState.AdvanceTime(2, isTraining: true);

        Assert.Equal(2, gameState.CurrentPartner.CareMistakes);
    }

    [Fact]
    public void AdvanceTime_WithNoCurrentPartner_StillAdvancesTheWorldClock()
    {
        var gameState = new GameState();

        gameState.AdvanceTime(5);

        Assert.Equal(5, gameState.CurrentTime.HourOfDay);
    }
}
