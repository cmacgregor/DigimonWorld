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
    public void Sleep_AdvancesWorldClockToTheFixedWakeUpHour_ButBumpsThePartnersStageTimerByAFixedAmount()
    {
        var gameState = new GameState
        {
            CurrentTime = { HourOfDay = 20, MinuteOfHour = 0 }, // 20:00 -> 7:00 next day = 11 real hours
            CurrentPartner = new PartnerDigimon { Level = DigimonLevelEnum.Rookie, Hunger = { Gauge = 1 } },
        };

        gameState.Sleep();

        Assert.Equal(WorldTime.WakeUpHour, gameState.CurrentTime.HourOfDay);
        Assert.Equal(9, gameState.CurrentPartner.HoursInCurrentStage); // fixed Rookie+ bump, not the real 11h
        Assert.Equal(0, gameState.CurrentPartner.CareMistakes); // neglect timer frozen while asleep
    }

    [Fact]
    public void Inventory_IsAPlayerLevelResource_SharedAlongsideBits()
    {
        var gameState = new GameState();

        gameState.Inventory.AddItem(itemId: 1, quantity: 3);

        Assert.Equal(3, gameState.Inventory.GetQuantity(1));
    }
}
