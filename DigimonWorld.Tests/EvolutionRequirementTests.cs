using Xunit;

public class EvolutionRequirementTests
{
    private static PartnerDigimon MakePartner()
    {
        return new PartnerDigimon();
    }

    [Fact]
    public void Fails_WhenHoursInCurrentStageBelowMinimum_RegardlessOfOtherConditions()
    {
        var partner = MakePartner();
        partner.HoursInCurrentStage = 1;
        partner.MinHoursInCurrentStage = 72;
        partner.Attack = 999;
        partner.Defense = 999;
        partner.Speed = 999;
        partner.Brains = 999;
        partner.Happiness = 999;
        partner.Discipline = 999;

        var requirement = new EvolutionRequirement
        {
            MinAttack = 1,
            HappinessThreshold = 1,
            DisciplineThreshold = 1,
        };

        Assert.False(requirement.IsMetBy(partner));
    }

    [Fact]
    public void Passes_WhenAtLeastThreeConfiguredConditionsAreSatisfied()
    {
        var partner = MakePartner();
        partner.HoursInCurrentStage = 100;
        partner.MinHoursInCurrentStage = 72;
        partner.Attack = 100;
        partner.Defense = 100;
        partner.Speed = 100;
        partner.Brains = 100;
        partner.CareMistakes = 0;
        partner.Discipline = 95;
        // Weight and techs learned deliberately left unmet.
        partner.Weight = 999;

        var requirement = new EvolutionRequirement
        {
            MinAttack = 100,
            MinDefense = 100,
            MinSpeed = 100,
            MinBrains = 100,
            MaxCareMistakes = 0,
            DisciplineThreshold = 90,
            MinWeight = 25,
            MaxWeight = 35,
            MinTechsLearned = 36,
        };

        // 3 of 5 configured conditions met: stat check, care mistakes, discipline.
        Assert.True(requirement.IsMetBy(partner));
    }

    [Fact]
    public void Fails_WhenFewerThanRequiredConditionsAreSatisfied()
    {
        var partner = MakePartner();
        partner.HoursInCurrentStage = 100;
        partner.MinHoursInCurrentStage = 72;
        partner.Attack = 100;
        partner.Defense = 100;
        partner.Speed = 100;
        partner.Brains = 100;
        partner.CareMistakes = 0;
        partner.Discipline = 0;
        partner.Weight = 999;

        var requirement = new EvolutionRequirement
        {
            MinAttack = 100,
            MinDefense = 100,
            MinSpeed = 100,
            MinBrains = 100,
            MaxCareMistakes = 0,
            DisciplineThreshold = 90,
            MinWeight = 25,
            MaxWeight = 35,
            MinTechsLearned = 36,
        };

        // Only 2 of 5 configured conditions met: stat check, care mistakes.
        Assert.False(requirement.IsMetBy(partner));
    }

    [Fact]
    public void StatCheck_CanGateOnASingleStat()
    {
        var partner = MakePartner();
        partner.Speed = 80;
        partner.Attack = 0;
        partner.Defense = 0;
        partner.Brains = 0;

        var requirement = new EvolutionRequirement { MinSpeed = 80 };

        Assert.True(EvaluateSingleCondition(requirement, partner));
    }

    [Fact]
    public void StatCheck_RequiresEveryConfiguredStatToPass()
    {
        var partner = MakePartner();
        partner.Attack = 100;
        partner.Defense = 100;
        partner.Speed = 100;
        partner.Brains = 99;

        var requirement = new EvolutionRequirement
        {
            MinAttack = 100,
            MinDefense = 100,
            MinSpeed = 100,
            MinBrains = 100,
        };

        Assert.False(EvaluateSingleCondition(requirement, partner));
    }

    [Fact]
    public void StatCheck_DoesNotCountWhenNoStatFieldsAreConfigured()
    {
        var partner = MakePartner();
        partner.Attack = 9999;

        var requirement = new EvolutionRequirement();

        Assert.False(EvaluateSingleCondition(requirement, partner));
    }

    [Fact]
    public void WeightRange_SatisfiedOnlyWithinBothBounds()
    {
        var requirement = new EvolutionRequirement { MinWeight = 25, MaxWeight = 35 };

        var tooLight = MakePartner();
        tooLight.Weight = 10;
        var justRight = MakePartner();
        justRight.Weight = 30;
        var tooHeavy = MakePartner();
        tooHeavy.Weight = 40;

        Assert.False(EvaluateSingleCondition(requirement, tooLight));
        Assert.True(EvaluateSingleCondition(requirement, justRight));
        Assert.False(EvaluateSingleCondition(requirement, tooHeavy));
    }

    [Fact]
    public void CareMistakesRange_SupportsBothAFloorAndACeiling()
    {
        var requirement = new EvolutionRequirement { MinCareMistakes = 1, MaxCareMistakes = 3 };

        var tooFew = MakePartner();
        tooFew.CareMistakes = 0;
        var withinRange = MakePartner();
        withinRange.CareMistakes = 2;
        var tooMany = MakePartner();
        tooMany.CareMistakes = 4;

        Assert.False(EvaluateSingleCondition(requirement, tooFew));
        Assert.True(EvaluateSingleCondition(requirement, withinRange));
        Assert.False(EvaluateSingleCondition(requirement, tooMany));
    }

    [Fact]
    public void BattlesFoughtRange_SupportsBothAFloorAndACeiling()
    {
        var requirement = new EvolutionRequirement { MinBattlesFought = 10, MaxBattlesFought = 50 };

        var tooFew = MakePartner();
        tooFew.BattlesFought = 5;
        var withinRange = MakePartner();
        withinRange.BattlesFought = 20;
        var tooMany = MakePartner();
        tooMany.BattlesFought = 60;

        Assert.False(EvaluateSingleCondition(requirement, tooFew));
        Assert.True(EvaluateSingleCondition(requirement, withinRange));
        Assert.False(EvaluateSingleCondition(requirement, tooMany));
    }

    [Fact]
    public void SpeciesBonus_IsJustOneNormalCondition_NotAFreePass()
    {
        var partner = MakePartner();
        partner.HoursInCurrentStage = 100;
        partner.MinHoursInCurrentStage = 72;
        partner.SpeciesId = 42;

        var requirement = new EvolutionRequirement { SpeciesBonusId = 42 };

        // Matching the bonus species satisfies only 1 of the 3 required conditions.
        Assert.False(requirement.IsMetBy(partner));
    }

    [Fact]
    public void SpeciesBonus_CountsTowardRequiredConditionsAlongsideOthers()
    {
        var partner = MakePartner();
        partner.HoursInCurrentStage = 100;
        partner.MinHoursInCurrentStage = 72;
        partner.SpeciesId = 42;
        partner.Happiness = 100;
        partner.Discipline = 100;

        var requirement = new EvolutionRequirement
        {
            SpeciesBonusId = 42,
            HappinessThreshold = 90,
            DisciplineThreshold = 90,
        };

        Assert.True(requirement.IsMetBy(partner));
    }

    [Fact]
    public void SpeciesBonus_DoesNotSatisfyOtherPartnersOfADifferentSpecies()
    {
        var partner = MakePartner();
        partner.HoursInCurrentStage = 100;
        partner.MinHoursInCurrentStage = 72;
        partner.SpeciesId = 7;
        partner.Happiness = 100;
        partner.Discipline = 100;

        var requirement = new EvolutionRequirement
        {
            SpeciesBonusId = 42,
            HappinessThreshold = 90,
            DisciplineThreshold = 90,
        };

        // Only 2 of the 3 required conditions met - the species bonus doesn't apply.
        Assert.False(requirement.IsMetBy(partner));
    }

    // A requirement with only one condition configured needs RequiredConditionCount (3)
    // satisfied conditions to pass, so it can never pass on its own. To test a single
    // condition in isolation, we add two always-true throwaway conditions and confirm
    // IsMetBy flips from false to true depending on whether the condition under test
    // is also satisfied.
    private static bool EvaluateSingleCondition(EvolutionRequirement requirement, PartnerDigimon partner)
    {
        partner.HoursInCurrentStage = System.Math.Max(partner.HoursInCurrentStage, partner.MinHoursInCurrentStage);
        partner.Happiness = 100;
        partner.Discipline = 100;
        requirement.HappinessThreshold = 0;
        requirement.DisciplineThreshold = 0;

        return requirement.IsMetBy(partner);
    }
}
