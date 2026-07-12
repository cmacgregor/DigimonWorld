using Xunit;

public class PartnerDigimonTests
{
    [Fact]
    public void GetEligibleEvolution_ReturnsFirstSatisfiedRequirementInPriorityOrder()
    {
        var partner = new PartnerDigimon
        {
            HoursInCurrentStage = 100,
            MinHoursInCurrentStage = 0,
            Happiness = 100,
            Discipline = 100,
        };

        var lowerPriorityButAlsoEligible = new EvolutionRequirement
        {
            TargetName = "Greymon",
            HappinessThreshold = 0,
            DisciplineThreshold = 0,
            MinCareMistakes = 0,
        };
        var higherPriorityAndEligible = new EvolutionRequirement
        {
            TargetName = "Tyrannomon",
            HappinessThreshold = 0,
            DisciplineThreshold = 0,
            MinCareMistakes = 0,
        };

        partner.PossibleEvolutions.Add(higherPriorityAndEligible);
        partner.PossibleEvolutions.Add(lowerPriorityButAlsoEligible);

        var result = partner.GetEligibleEvolution();

        Assert.Same(higherPriorityAndEligible, result);
    }

    [Fact]
    public void GetEligibleEvolution_SkipsUnsatisfiedRequirementsAheadOfAnEligibleOne()
    {
        var partner = new PartnerDigimon
        {
            HoursInCurrentStage = 100,
            MinHoursInCurrentStage = 0,
            Happiness = 0,
            Discipline = 0,
        };

        var unmetRequirement = new EvolutionRequirement
        {
            TargetName = "Greymon",
            HappinessThreshold = 90,
            DisciplineThreshold = 90,
            MinCareMistakes = 0,
        };
        var metRequirement = new EvolutionRequirement
        {
            TargetName = "Numemon",
            HappinessThreshold = 0,
            DisciplineThreshold = 0,
            MinCareMistakes = 0,
        };

        partner.PossibleEvolutions.Add(unmetRequirement);
        partner.PossibleEvolutions.Add(metRequirement);

        var result = partner.GetEligibleEvolution();

        Assert.Same(metRequirement, result);
    }

    [Fact]
    public void GetEligibleEvolution_ReturnsNullWhenNothingIsSatisfied()
    {
        var partner = new PartnerDigimon
        {
            HoursInCurrentStage = 0,
            MinHoursInCurrentStage = 72,
        };

        partner.PossibleEvolutions.Add(new EvolutionRequirement { TargetName = "Greymon" });

        Assert.Null(partner.GetEligibleEvolution());
    }

    [Fact]
    public void AdvanceTime_OnFullHours_IncrementsStageTimerAndSleepGauge_CountsDownHungerGaugeByMinutes()
    {
        var partner = new PartnerDigimon
        {
            HoursInCurrentStage = 10,
            Hunger = { Gauge = 200 },
            Sleep = { Gauge = 2 },
        };

        partner.AdvanceTime(3 * PartnerDigimon.MinutesPerHour); // 3 hours

        Assert.Equal(13, partner.HoursInCurrentStage);
        Assert.Equal(20, partner.Hunger.Gauge); // 200 - 180 minutes
        Assert.Equal(5, partner.Sleep.Gauge);
    }

    [Fact]
    public void AdvanceTime_WithinTheSameHour_DoesNotYetIncrementStageTimerOrSleepGauge()
    {
        var partner = new PartnerDigimon
        {
            HoursInCurrentStage = 10,
            Sleep = { Gauge = 2 },
            MinuteOfHour = 10,
        };

        partner.AdvanceTime(30);

        Assert.Equal(10, partner.HoursInCurrentStage);
        Assert.Equal(2, partner.Sleep.Gauge);
        Assert.Equal(40, partner.MinuteOfHour);
    }

    [Fact]
    public void AdvanceTime_AddsHungerNeglectCareMistakes_ToThePartnersTotal()
    {
        var partner = new PartnerDigimon { Hunger = { Gauge = 1 }, IsTraining = true };

        partner.AdvanceTime(91); // crosses the neglect threshold while training

        Assert.Equal(2, partner.CareMistakes);
    }

    [Fact]
    public void AdvanceTime_SubtractsEnergyStarvationLoss_FromWeight()
    {
        var partner = new PartnerDigimon
        {
            Hunger = { Gauge = 0 },
            Energy = { Gauge = 0 },
            Weight = 500,
        };

        partner.AdvanceTime(10); // Energy already depleted and hungry -> 1 gram lost

        Assert.Equal(499, partner.Weight);
    }

    [Fact]
    public void AdvanceTimeAsleep_LosesWeightByTheHour_WhenHungry()
    {
        var partner = new PartnerDigimon { Hunger = { Gauge = 0 }, Weight = 500 };

        partner.AdvanceTimeAsleep(9 * PartnerDigimon.MinutesPerHour);

        Assert.Equal(491, partner.Weight); // 500 - 9g for 9 hours asleep while hungry
    }

    [Fact]
    public void AdvanceTimeAsleep_FreezesTheHungerNeglectCareMistakeTimer()
    {
        var partner = new PartnerDigimon { Hunger = { Gauge = 1 } };

        partner.AdvanceTimeAsleep(91);

        Assert.False(partner.Hunger.CareMistakeApplied);
        Assert.Equal(0, partner.CareMistakes);
        Assert.Equal(-90, partner.Hunger.Gauge); // not reset, since neglect never fires
    }

    [Fact]
    public void AdvanceTimeAsleep_StillAdvancesStageTimerAndSleepGauge()
    {
        var partner = new PartnerDigimon { HoursInCurrentStage = 0, Sleep = { Gauge = 0 } };

        partner.AdvanceTimeAsleep(90); // 1h30m

        Assert.Equal(1, partner.HoursInCurrentStage);
        Assert.Equal(1, partner.Sleep.Gauge);
        Assert.Equal(30, partner.MinuteOfHour);
    }
}
