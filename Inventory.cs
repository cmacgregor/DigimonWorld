using System.Collections.Generic;

// Player-level stockpile of items, shared across whichever partner is
// active - lives on GameState alongside Bits. Stacks by ItemId rather
// than tracking individual item instances.
public class Inventory
{
    private readonly Dictionary<int, int> _quantities = new();

    public int GetQuantity(int itemId) => _quantities.TryGetValue(itemId, out var quantity) ? quantity : 0;

    public bool HasItem(int itemId, int quantity = 1) => GetQuantity(itemId) >= quantity;

    public void AddItem(int itemId, int quantity = 1)
    {
        _quantities[itemId] = GetQuantity(itemId) + quantity;
    }

    // Returns false (and leaves the inventory unchanged) if there isn't
    // enough of the item to remove.
    public bool RemoveItem(int itemId, int quantity = 1)
    {
        if (!HasItem(itemId, quantity))
        {
            return false;
        }

        _quantities[itemId] -= quantity;
        return true;
    }
}
