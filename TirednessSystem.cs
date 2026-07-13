// Tiredness builds up from 0 to 100 (unlike Hunger, which counts down).
// It's not ticked by elapsed time yet - training/battle-driven gain isn't
// modeled, same as the stat-gain formulas those would feed.
public class TirednessSystem
{
    public const int GaugeMax = 100;
    public const int TiredThreshold = 80;
    public const int OverworkedThreshold = 100;

    public int Gauge { get; set; }

    public TirednessEnum State
    {
        get
        {
            if (Gauge >= OverworkedThreshold) return TirednessEnum.Overworked;
            if (Gauge >= TiredThreshold) return TirednessEnum.Tired;
            return TirednessEnum.Rested;
        }
    }
}
