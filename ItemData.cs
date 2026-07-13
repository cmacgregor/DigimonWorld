// Static per-item data - just enough to identify and categorize an item.
// TODO: Category-specific effects (food's hunger/stat restore amount,
// medicine's cure effect, which evolution an Evolution item triggers)
// aren't modeled yet - those stay external/TODO until designed, same as
// the stat-gain formulas.
public class ItemData
{
    public int ItemId { get; set; }
    public string Name { get; set; }
    public ItemCategoryEnum Category { get; set; }
}
