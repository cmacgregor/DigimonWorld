using Xunit;

public class ItemDataTests
{
    [Fact]
    public void UsableIn_SupportsBattleOnlyItems()
    {
        var smokeBomb = new ItemData { ItemId = 1, Name = "Smoke Bomb", UsableIn = ItemUsageEnum.Battle };

        Assert.True(smokeBomb.UsableIn.HasFlag(ItemUsageEnum.Battle));
        Assert.False(smokeBomb.UsableIn.HasFlag(ItemUsageEnum.Feed));
    }

    [Fact]
    public void UsableIn_SupportsItemsUsableAsFoodAndInBattle()
    {
        var recoveryDisk = new ItemData { ItemId = 2, Name = "Recovery Disk", UsableIn = ItemUsageEnum.Feed | ItemUsageEnum.Battle };

        Assert.True(recoveryDisk.UsableIn.HasFlag(ItemUsageEnum.Feed));
        Assert.True(recoveryDisk.UsableIn.HasFlag(ItemUsageEnum.Battle));
    }
}
