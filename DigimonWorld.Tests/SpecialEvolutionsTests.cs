using Xunit;

public class SpecialEvolutionsTests
{
    [Fact]
    public void EvolveToSukamon_HalvesEveryStatButStillGrantsLifespan()
    {
        var partner = new PartnerDigimon
        {
            Attack = 100,
            Defense = 51,
            Lifespan = 200,
        };

        SpecialEvolutions.EvolveToSukamon(partner, new DigimonEvolutionData { SpeciesId = 999 });

        Assert.Equal(999, partner.SpeciesId);
        Assert.Equal(50, partner.Attack);
        Assert.Equal(25, partner.Defense); // 51 * 50 / 100, truncated
        Assert.Equal(200 + PartnerDigimon.LifespanGainOnEvolve, partner.Lifespan);
    }

    [Fact]
    public void EvolveToNumemon_ReducesEveryStatByTwentyPercentButStillGrantsLifespan()
    {
        var partner = new PartnerDigimon
        {
            Attack = 100,
            Lifespan = 200,
        };

        SpecialEvolutions.EvolveToNumemon(partner, new DigimonEvolutionData { SpeciesId = 998 });

        Assert.Equal(998, partner.SpeciesId);
        Assert.Equal(80, partner.Attack);
        Assert.Equal(200 + PartnerDigimon.LifespanGainOnEvolve, partner.Lifespan);
    }

    [Fact]
    public void CanEvolveToNanimon_RequiresRookieLevelAndZeroHappinessAndDiscipline()
    {
        var eligible = new PartnerDigimon
        {
            Level = DigimonLevelEnum.Rookie,
            Happiness = 0,
            Discipline = 0,
        };
        var wrongLevel = new PartnerDigimon
        {
            Level = DigimonLevelEnum.Champion,
            Happiness = 0,
            Discipline = 0,
        };
        var happinessNotZero = new PartnerDigimon
        {
            Level = DigimonLevelEnum.Rookie,
            Happiness = 1,
            Discipline = 0,
        };

        Assert.True(SpecialEvolutions.CanEvolveToNanimon(eligible));
        Assert.False(SpecialEvolutions.CanEvolveToNanimon(wrongLevel));
        Assert.False(SpecialEvolutions.CanEvolveToNanimon(happinessNotZero));
    }

    [Fact]
    public void EvolveToNanimon_ChangesSpeciesWithNoStatChangeButGrantsLifespan_AndKeepsStageTimer()
    {
        var partner = new PartnerDigimon
        {
            Attack = 100,
            Lifespan = 200,
            HoursInCurrentStage = 50,
        };

        SpecialEvolutions.EvolveToNanimon(partner, new DigimonEvolutionData
        {
            SpeciesId = 997,
            ReferenceAttack = 999,
        });

        Assert.Equal(997, partner.SpeciesId);
        Assert.Equal(100, partner.Attack); // unchanged
        Assert.Equal(200 + PartnerDigimon.LifespanGainOnEvolve, partner.Lifespan);
        Assert.Equal(50, partner.HoursInCurrentStage); // same-level - timer not reset
    }

    [Fact]
    public void CanEvolveToKunemon_RequiresInTrainingInThePlaceholderArea()
    {
        var eligible = new PartnerDigimon { Level = DigimonLevelEnum.InTraining, CurrentLocation = LocationEnum.PlaceholderArea };
        var wrongLevel = new PartnerDigimon { Level = DigimonLevelEnum.Rookie, CurrentLocation = LocationEnum.PlaceholderArea };
        var wrongLocation = new PartnerDigimon { Level = DigimonLevelEnum.InTraining, CurrentLocation = LocationEnum.None };

        Assert.True(SpecialEvolutions.CanEvolveToKunemon(eligible));
        Assert.False(SpecialEvolutions.CanEvolveToKunemon(wrongLevel));
        Assert.False(SpecialEvolutions.CanEvolveToKunemon(wrongLocation));
    }

    [Fact]
    public void CanEvolveToBakemon_ExcludesPenguinmonButAllowsOtherRookies()
    {
        const int penguinmonId = 42;

        var penguinmon = new PartnerDigimon { Level = DigimonLevelEnum.Rookie, SpeciesId = penguinmonId };
        var otherRookie = new PartnerDigimon { Level = DigimonLevelEnum.Rookie, SpeciesId = 7 };
        var wrongLevel = new PartnerDigimon { Level = DigimonLevelEnum.Champion, SpeciesId = 7 };

        Assert.False(SpecialEvolutions.CanEvolveToBakemon(penguinmon, penguinmonId));
        Assert.True(SpecialEvolutions.CanEvolveToBakemon(otherRookie, penguinmonId));
        Assert.False(SpecialEvolutions.CanEvolveToBakemon(wrongLevel, penguinmonId));
    }

    [Fact]
    public void CanEvolveToDevimon_RequiresAngemonSpecificallyWithLowDiscipline()
    {
        const int angemonId = 99;

        var eligible = new PartnerDigimon { SpeciesId = angemonId, Discipline = 50 };
        var disciplineTooHigh = new PartnerDigimon { SpeciesId = angemonId, Discipline = 51 };
        var wrongSpecies = new PartnerDigimon { SpeciesId = 7, Discipline = 0 };

        Assert.True(SpecialEvolutions.CanEvolveToDevimon(eligible, angemonId));
        Assert.False(SpecialEvolutions.CanEvolveToDevimon(disciplineTooHigh, angemonId));
        Assert.False(SpecialEvolutions.CanEvolveToDevimon(wrongSpecies, angemonId));
    }

    [Fact]
    public void CanEvolveToAirdramon_AllowsEitherSeadramonOrBirdramonAtFullDisciplineAndHappiness()
    {
        const int seadramonId = 10;
        const int birdramonId = 11;

        var seadramonEligible = new PartnerDigimon { SpeciesId = seadramonId, Discipline = 100, Happiness = 100 };
        var birdramonEligible = new PartnerDigimon { SpeciesId = birdramonId, Discipline = 100, Happiness = 100 };
        var wrongSpecies = new PartnerDigimon { SpeciesId = 7, Discipline = 100, Happiness = 100 };
        var happinessTooLow = new PartnerDigimon { SpeciesId = seadramonId, Discipline = 100, Happiness = 99 };

        Assert.True(SpecialEvolutions.CanEvolveToAirdramon(seadramonEligible, seadramonId, birdramonId));
        Assert.True(SpecialEvolutions.CanEvolveToAirdramon(birdramonEligible, seadramonId, birdramonId));
        Assert.False(SpecialEvolutions.CanEvolveToAirdramon(wrongSpecies, seadramonId, birdramonId));
        Assert.False(SpecialEvolutions.CanEvolveToAirdramon(happinessTooLow, seadramonId, birdramonId));
    }

    [Fact]
    public void CanEvolveToAirdramon_RequiresTirednessGaugeExactlyZero()
    {
        const int seadramonId = 10;
        const int birdramonId = 11;

        var tired = new PartnerDigimon
        {
            SpeciesId = seadramonId,
            Discipline = 100,
            Happiness = 100,
            Tiredness = { Gauge = 80 },
        };
        var restedButNotZero = new PartnerDigimon
        {
            SpeciesId = seadramonId,
            Discipline = 100,
            Happiness = 100,
            Tiredness = { Gauge = 1 },
        };

        Assert.False(SpecialEvolutions.CanEvolveToAirdramon(tired, seadramonId, birdramonId));
        Assert.False(SpecialEvolutions.CanEvolveToAirdramon(restedButNotZero, seadramonId, birdramonId));
    }

    [Fact]
    public void CanEvolveToCoelamon_AllowsEitherWhamonOrShellmonAtExactly200Hours()
    {
        const int whamonId = 20;
        const int shellmonId = 21;

        var whamonEligible = new PartnerDigimon { SpeciesId = whamonId, HoursInCurrentStage = 200 };
        var shellmonEligible = new PartnerDigimon { SpeciesId = shellmonId, HoursInCurrentStage = 200 };
        var wrongSpecies = new PartnerDigimon { SpeciesId = 7, HoursInCurrentStage = 200 };
        var wrongHour = new PartnerDigimon { SpeciesId = whamonId, HoursInCurrentStage = 201 };

        Assert.True(SpecialEvolutions.CanEvolveToCoelamon(whamonEligible, whamonId, shellmonId));
        Assert.True(SpecialEvolutions.CanEvolveToCoelamon(shellmonEligible, whamonId, shellmonId));
        Assert.False(SpecialEvolutions.CanEvolveToCoelamon(wrongSpecies, whamonId, shellmonId));
        Assert.False(SpecialEvolutions.CanEvolveToCoelamon(wrongHour, whamonId, shellmonId));
    }

    [Fact]
    public void CanEvolveToNinjamon_RequiresVegimonFullDisciplineAndOver50BattlesWon()
    {
        const int vegimonId = 30;

        var eligible = new PartnerDigimon { SpeciesId = vegimonId, Discipline = 100, BattlesWon = 51 };
        var notEnoughBattles = new PartnerDigimon { SpeciesId = vegimonId, Discipline = 100, BattlesWon = 50 };
        var disciplineNotFull = new PartnerDigimon { SpeciesId = vegimonId, Discipline = 99, BattlesWon = 51 };
        var wrongSpecies = new PartnerDigimon { SpeciesId = 7, Discipline = 100, BattlesWon = 51 };

        Assert.True(SpecialEvolutions.CanEvolveToNinjamon(eligible, vegimonId));
        Assert.False(SpecialEvolutions.CanEvolveToNinjamon(notEnoughBattles, vegimonId));
        Assert.False(SpecialEvolutions.CanEvolveToNinjamon(disciplineNotFull, vegimonId));
        Assert.False(SpecialEvolutions.CanEvolveToNinjamon(wrongSpecies, vegimonId));
    }

    [Fact]
    public void CanEvolveToMonochromon_RequiresDrimogemonFullDisciplineAndDefenseAtLeast500()
    {
        const int drimogemonId = 40;

        var eligible = new PartnerDigimon { SpeciesId = drimogemonId, Discipline = 100, Defense = 500 };
        var defenseTooLow = new PartnerDigimon { SpeciesId = drimogemonId, Discipline = 100, Defense = 499 };
        var disciplineNotFull = new PartnerDigimon { SpeciesId = drimogemonId, Discipline = 99, Defense = 500 };
        var wrongSpecies = new PartnerDigimon { SpeciesId = 7, Discipline = 100, Defense = 500 };

        Assert.True(SpecialEvolutions.CanEvolveToMonochromon(eligible, drimogemonId));
        Assert.False(SpecialEvolutions.CanEvolveToMonochromon(defenseTooLow, drimogemonId));
        Assert.False(SpecialEvolutions.CanEvolveToMonochromon(disciplineNotFull, drimogemonId));
        Assert.False(SpecialEvolutions.CanEvolveToMonochromon(wrongSpecies, drimogemonId));
    }

    [Fact]
    public void CanEvolveToVademonFromPraiseOrScold_RequiresChampionAtOrPast240Hours()
    {
        var eligible = new PartnerDigimon { Level = DigimonLevelEnum.Champion, HoursInCurrentStage = 240 };
        var pastThreshold = new PartnerDigimon { Level = DigimonLevelEnum.Champion, HoursInCurrentStage = 300 };
        var tooEarly = new PartnerDigimon { Level = DigimonLevelEnum.Champion, HoursInCurrentStage = 239 };
        var wrongLevel = new PartnerDigimon { Level = DigimonLevelEnum.Rookie, HoursInCurrentStage = 240 };

        Assert.True(SpecialEvolutions.CanEvolveToVademonFromPraiseOrScold(eligible));
        Assert.True(SpecialEvolutions.CanEvolveToVademonFromPraiseOrScold(pastThreshold));
        Assert.False(SpecialEvolutions.CanEvolveToVademonFromPraiseOrScold(tooEarly));
        Assert.False(SpecialEvolutions.CanEvolveToVademonFromPraiseOrScold(wrongLevel));
    }

    [Fact]
    public void CanEvolveToVademonFromTimeout_RequiresChampionAt360HoursWithNoEligibleEvolution()
    {
        var eligibleWithNoOptions = new PartnerDigimon { Level = DigimonLevelEnum.Champion, HoursInCurrentStage = 360 };

        var hasAnEligibleEvolution = new PartnerDigimon { Level = DigimonLevelEnum.Champion, HoursInCurrentStage = 360 };
        hasAnEligibleEvolution.PossibleEvolutions.Add(new EvolutionRequirement
        {
            TargetName = "SomeUltimate",
            HappinessThreshold = 0,
            DisciplineThreshold = 0,
            MaxCareMistakes = 0,
        });

        var tooEarly = new PartnerDigimon { Level = DigimonLevelEnum.Champion, HoursInCurrentStage = 359 };
        var wrongLevel = new PartnerDigimon { Level = DigimonLevelEnum.Rookie, HoursInCurrentStage = 360 };

        Assert.True(SpecialEvolutions.CanEvolveToVademonFromTimeout(eligibleWithNoOptions));
        Assert.False(SpecialEvolutions.CanEvolveToVademonFromTimeout(hasAnEligibleEvolution));
        Assert.False(SpecialEvolutions.CanEvolveToVademonFromTimeout(tooEarly));
        Assert.False(SpecialEvolutions.CanEvolveToVademonFromTimeout(wrongLevel));
    }

    [Fact]
    public void CanEvolveToPhoenixmon_RequiresKokatorimonSpecifically()
    {
        const int kokatorimonId = 50;

        var eligible = new PartnerDigimon { SpeciesId = kokatorimonId };
        var wrongSpecies = new PartnerDigimon { SpeciesId = 7 };

        Assert.True(SpecialEvolutions.CanEvolveToPhoenixmon(eligible, kokatorimonId));
        Assert.False(SpecialEvolutions.CanEvolveToPhoenixmon(wrongSpecies, kokatorimonId));
    }

    [Fact]
    public void CanEvolveToSkullgreymon_AllowsEitherMetalGreymonOrMegadramon()
    {
        const int metalGreymonId = 60;
        const int megadramonId = 61;

        var metalGreymonEligible = new PartnerDigimon { SpeciesId = metalGreymonId };
        var megadramonEligible = new PartnerDigimon { SpeciesId = megadramonId };
        var wrongSpecies = new PartnerDigimon { SpeciesId = 7 };

        Assert.True(SpecialEvolutions.CanEvolveToSkullgreymon(metalGreymonEligible, metalGreymonId, megadramonId));
        Assert.True(SpecialEvolutions.CanEvolveToSkullgreymon(megadramonEligible, metalGreymonId, megadramonId));
        Assert.False(SpecialEvolutions.CanEvolveToSkullgreymon(wrongSpecies, metalGreymonId, megadramonId));
    }
}
