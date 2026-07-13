// Static per-item data - just enough to identify an item and which
// contexts it can be used in. UsableIn is a flag set, so an item can be
// e.g. Feed-only, Battle-only, or both at once.
// TODO: Usage-specific effects (feed's hunger/stat restore amount,
// battle's effect, which evolution an Evolution item triggers) aren't
// modeled yet - those stay external/TODO until designed, same as the
// stat-gain formulas.
public class ItemData
{
    public int ItemId { get; set; }
    public string Name { get; set; }
    public ItemUsageEnum UsableIn { get; set; }
}
