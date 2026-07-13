using Xunit;

public class DigimonEvolutionDataTests
{
    [Fact]
    public void CalculateStatGain_WhenAtOrBelowReference_IsHalfTheDifference()
    {
        Assert.Equal(25, DigimonEvolutionData.CalculateStatGain(currentStat: 50, referenceStat: 100));
        Assert.Equal(0, DigimonEvolutionData.CalculateStatGain(currentStat: 100, referenceStat: 100));
    }

    [Fact]
    public void CalculateStatGain_WhenAboveReference_IsOneTenthOfReference()
    {
        Assert.Equal(10, DigimonEvolutionData.CalculateStatGain(currentStat: 150, referenceStat: 100));
    }

    [Fact]
    public void Evolve_AppliesStatGainToEachStatIndependently()
    {
        var partner = new PartnerDigimon
        {
            Attack = 50,
            Defense = 150,
            Speed = 0,
            Brains = 0,
            MaxHP = 0,
            MaxMP = 0,
        };

        var greymonData = new DigimonEvolutionData
        {
            SpeciesId = 4,
            ReferenceAttack = 100,
            ReferenceDefense = 100,
            ReferenceSpeed = 100,
            ReferenceBrains = 100,
            ReferenceMaxHP = 100,
            ReferenceMaxMP = 100,
        };

        partner.Evolve(greymonData);

        Assert.Equal(4, partner.SpeciesId);
        Assert.Equal(75, partner.Attack); // 50 <= 100: 50 + (100-50)/2
        Assert.Equal(160, partner.Defense); // 150 > 100: 150 + 100/10
        Assert.Equal(50, partner.Speed); // 0 <= 100: 0 + (100-0)/2
    }

    [Fact]
    public void Evolve_ResetsHoursInCurrentStageAndRefreshesPossibleEvolutions()
    {
        var partner = new PartnerDigimon { HoursInCurrentStage = 100 };
        partner.PossibleEvolutions.Add(new EvolutionRequirement { TargetName = "Stale" });

        var newRequirement = new EvolutionRequirement { TargetName = "MetalGreymon" };
        var newSpeciesData = new DigimonEvolutionData { SpeciesId = 5 };
        newSpeciesData.PossibleEvolutions.Add(newRequirement);

        partner.Evolve(newSpeciesData);

        Assert.Equal(0, partner.HoursInCurrentStage);
        Assert.Single(partner.PossibleEvolutions);
        Assert.Same(newRequirement, partner.PossibleEvolutions[0]);
    }

    [Fact]
    public void Evolve_ResetsCareMistakesAndBattlesFoughtAndWon()
    {
        var partner = new PartnerDigimon
        {
            CareMistakes = 5,
            BattlesFought = 42,
            BattlesWon = 30,
        };

        partner.Evolve(new DigimonEvolutionData());

        Assert.Equal(0, partner.CareMistakes);
        Assert.Equal(0, partner.BattlesFought);
        Assert.Equal(0, partner.BattlesWon);
    }

    [Fact]
    public void Evolve_AlwaysGrantsAFlatLifespanBonus()
    {
        var partner = new PartnerDigimon { Lifespan = 200 };

        partner.Evolve(new DigimonEvolutionData());

        Assert.Equal(200 + PartnerDigimon.LifespanGainOnEvolve, partner.Lifespan);
    }

    [Fact]
    public void Evolve_SameLevelEvolution_AppliesNormalStatGainButKeepsStageTimer()
    {
        var partner = new PartnerDigimon
        {
            Attack = 50,
            HoursInCurrentStage = 50,
        };

        partner.Evolve(new DigimonEvolutionData { ReferenceAttack = 100 }, isSameLevelEvolution: true);

        Assert.Equal(75, partner.Attack); // normal gain formula still applies
        Assert.Equal(50, partner.HoursInCurrentStage); // same-level - timer not reset
    }

    [Fact]
    public void EvolveWithItem_ChangesSpeciesWithNoStatOrLifespanChange_WhenTargetIsInPossibleEvolutions()
    {
        var partner = new PartnerDigimon
        {
            Attack = 100,
            Defense = 100,
            Lifespan = 200,
        };
        partner.PossibleEvolutions.Add(new EvolutionRequirement { TargetDigimonId = 4 });

        var succeeded = partner.EvolveWithItem(new DigimonEvolutionData
        {
            SpeciesId = 4,
            ReferenceAttack = 999,
            ReferenceDefense = 999,
        });

        Assert.True(succeeded);
        Assert.Equal(4, partner.SpeciesId);
        Assert.Equal(100, partner.Attack);
        Assert.Equal(100, partner.Defense);
        Assert.Equal(200, partner.Lifespan);
    }

    [Fact]
    public void EvolveWithItem_FailsWhenTargetIsNotInPossibleEvolutions()
    {
        var partner = new PartnerDigimon { SpeciesId = 1 };
        partner.PossibleEvolutions.Add(new EvolutionRequirement { TargetDigimonId = 4 });

        var succeeded = partner.EvolveWithItem(new DigimonEvolutionData { SpeciesId = 999 });

        Assert.False(succeeded);
        Assert.Equal(1, partner.SpeciesId); // unchanged
    }
}
