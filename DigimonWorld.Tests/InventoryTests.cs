using Xunit;

public class InventoryTests
{
    [Fact]
    public void GetQuantity_IsZero_ForAnItemNeverAdded()
    {
        var inventory = new Inventory();

        Assert.Equal(0, inventory.GetQuantity(1));
    }

    [Fact]
    public void AddItem_IncreasesQuantity()
    {
        var inventory = new Inventory();

        inventory.AddItem(1, 3);

        Assert.Equal(3, inventory.GetQuantity(1));
    }

    [Fact]
    public void AddItem_StacksOntoExistingQuantity()
    {
        var inventory = new Inventory();

        inventory.AddItem(1, 3);
        inventory.AddItem(1, 2);

        Assert.Equal(5, inventory.GetQuantity(1));
    }

    [Fact]
    public void AddItem_DefaultsToOne()
    {
        var inventory = new Inventory();

        inventory.AddItem(1);

        Assert.Equal(1, inventory.GetQuantity(1));
    }

    [Fact]
    public void AddItem_TracksDifferentItemIdsSeparately()
    {
        var inventory = new Inventory();

        inventory.AddItem(1, 3);
        inventory.AddItem(2, 7);

        Assert.Equal(3, inventory.GetQuantity(1));
        Assert.Equal(7, inventory.GetQuantity(2));
    }

    [Fact]
    public void HasItem_ReflectsWhetherEnoughQuantityIsAvailable()
    {
        var inventory = new Inventory();
        inventory.AddItem(1, 3);

        Assert.True(inventory.HasItem(1));
        Assert.True(inventory.HasItem(1, 3));
        Assert.False(inventory.HasItem(1, 4));
        Assert.False(inventory.HasItem(2));
    }

    [Fact]
    public void RemoveItem_DecreasesQuantity_WhenEnoughIsAvailable()
    {
        var inventory = new Inventory();
        inventory.AddItem(1, 3);

        var removed = inventory.RemoveItem(1, 2);

        Assert.True(removed);
        Assert.Equal(1, inventory.GetQuantity(1));
    }

    [Fact]
    public void RemoveItem_FailsAndLeavesQuantityUnchanged_WhenNotEnoughIsAvailable()
    {
        var inventory = new Inventory();
        inventory.AddItem(1, 1);

        var removed = inventory.RemoveItem(1, 2);

        Assert.False(removed);
        Assert.Equal(1, inventory.GetQuantity(1));
    }
}
