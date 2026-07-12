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
            HungerGauge = 200,
            SleepGauge = 2,
        };

        partner.AdvanceTime(3 * PartnerDigimon.MinutesPerHour); // 3 hours

        Assert.Equal(13, partner.HoursInCurrentStage);
        Assert.Equal(20, partner.HungerGauge); // 200 - 180 minutes
        Assert.Equal(5, partner.SleepGauge);
    }

    [Fact]
    public void AdvanceTime_WithinTheSameHour_DoesNotYetIncrementStageTimerOrSleepGauge()
    {
        var partner = new PartnerDigimon
        {
            HoursInCurrentStage = 10,
            SleepGauge = 2,
            MinuteOfHour = 10,
        };

        partner.AdvanceTime(30);

        Assert.Equal(10, partner.HoursInCurrentStage);
        Assert.Equal(2, partner.SleepGauge);
        Assert.Equal(40, partner.MinuteOfHour);
    }

    [Fact]
    public void Hungry_IsTrueOnceHungerGaugeReachesZeroOrBelow()
    {
        var notHungry = new PartnerDigimon { HungerGauge = 1 };
        var justHungry = new PartnerDigimon { HungerGauge = 0 };
        var pastHungry = new PartnerDigimon { HungerGauge = -5 };

        Assert.False(notHungry.Hungry);
        Assert.True(justHungry.Hungry);
        Assert.True(pastHungry.Hungry);
    }

    [Fact]
    public void AdvanceTime_AppliesOneCareMistake_WhenHungerNeglectCrossesOneAndAHalfHours()
    {
        var partner = new PartnerDigimon { HungerGauge = 1 };

        // Crosses from HungerGauge=1 to -90 in one tick (91 minutes).
        partner.AdvanceTime(91);

        Assert.True(partner.HungerCareMistakeApplied);
        Assert.Equal(1, partner.CareMistakes);
    }

    [Fact]
    public void AdvanceTime_ResetsHungerGauge_WhenNeglectCareMistakeFires()
    {
        var partner = new PartnerDigimon { HungerGauge = 1 };

        partner.AdvanceTime(91);

        Assert.Equal(PartnerDigimon.HungerGaugeResetValueAfterNeglect, partner.HungerGauge);
    }

    [Fact]
    public void AdvanceTime_AppliesTwoCareMistakes_WhenHungerNeglectCrossesWhileTraining()
    {
        var partner = new PartnerDigimon { HungerGauge = 1 };

        partner.AdvanceTime(91, isTraining: true);

        Assert.True(partner.HungerCareMistakeApplied);
        Assert.Equal(2, partner.CareMistakes);
    }

    [Fact]
    public void AdvanceTime_OnlyAppliesTheHungerCareMistakeOnce()
    {
        var partner = new PartnerDigimon { HungerGauge = 1 };

        partner.AdvanceTime(91); // crosses the threshold, applies 1 mistake
        partner.AdvanceTime(300); // still neglected, well past the threshold

        Assert.Equal(1, partner.CareMistakes);
    }

    [Fact]
    public void AdvanceTime_DoesNotApplyACareMistake_WhenStillWithinTheNeglectWindow()
    {
        var partner = new PartnerDigimon { HungerGauge = 1 };

        // Hungry (gauge <= 0) but not yet 90 minutes past it.
        partner.AdvanceTime(50);

        Assert.False(partner.HungerCareMistakeApplied);
        Assert.Equal(0, partner.CareMistakes);
    }
}
