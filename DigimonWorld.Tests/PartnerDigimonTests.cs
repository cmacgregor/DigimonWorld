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
    public void AdvanceTime_IncrementsStageTimerAndHungerAndSleepGauges()
    {
        var partner = new PartnerDigimon
        {
            HoursInCurrentStage = 10,
            HungerGauge = 5,
            SleepGauge = 2,
        };

        partner.AdvanceTime(3);

        Assert.Equal(13, partner.HoursInCurrentStage);
        Assert.Equal(8, partner.HungerGauge);
        Assert.Equal(5, partner.SleepGauge);
    }
}
