using Xunit;

public class TechListTests
{
    private static Tech MakeTech(string name) => new Tech { Name = name };

    [Fact]
    public void TryAdd_SucceedsUpToCapacity()
    {
        var techList = new TechList();

        Assert.True(techList.TryAdd(MakeTech("Pepper Breath")));
        Assert.True(techList.TryAdd(MakeTech("Spitfire Blast")));
        Assert.True(techList.TryAdd(MakeTech("Baby Flame")));

        Assert.Equal(3, techList.Count);
    }

    [Fact]
    public void TryAdd_FailsPastCapacity()
    {
        var techList = new TechList();
        techList.TryAdd(MakeTech("Pepper Breath"));
        techList.TryAdd(MakeTech("Spitfire Blast"));
        techList.TryAdd(MakeTech("Baby Flame"));

        var addedFourth = techList.TryAdd(MakeTech("Nova Blast"));

        Assert.False(addedFourth);
        Assert.Equal(TechList.Capacity, techList.Count);
    }

    [Fact]
    public void Capacity_IsThree()
    {
        Assert.Equal(3, TechList.Capacity);
    }
}
