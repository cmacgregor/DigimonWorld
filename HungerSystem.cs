// Hunger is a timer that counts down, not a meter that builds up (unlike
// Tiredness/Potty) - once it hits 0, Hungry goes true. Left neglected past
// the reset threshold, it incurs a care mistake.
public class HungerSystem
{
    public const int GaugeMax = 480; // placeholder: ~8h before Hungry
    public const int NeglectCareMistakeMinutes = 90; // 1.5h past Hungry
    public const int NeglectCareMistakesWhileTraining = 2;
    public const int GaugeResetValueAfterNeglect = 100; // placeholder

    public int Gauge { get; set; }
    // Set once the first time Gauge crosses the neglect threshold, so the
    // care mistake fires once per hungry episode, not every tick. Feeding
    // (not built yet) is expected to reset this back to false along with
    // Gauge.
    public bool CareMistakeApplied { get; set; }

    public bool Hungry => Gauge <= 0;

    // Advances the countdown by minutes. Returns the care mistakes
    // incurred from neglect this tick (0 if none) - CareMistakes itself
    // lives on PartnerDigimon since multiple systems can contribute to it.
    // isSleeping freezes the neglect timer entirely: the gauge still
    // counts down, but no mistake fires and it isn't reset.
    public int Advance(int minutes, bool isTraining, bool isSleeping)
    {
        var previousGauge = Gauge;
        Gauge -= minutes;

        if (isSleeping)
        {
            return 0;
        }

        var neglectThreshold = -NeglectCareMistakeMinutes;
        if (!CareMistakeApplied && previousGauge > neglectThreshold && Gauge <= neglectThreshold)
        {
            CareMistakeApplied = true;
            Gauge = GaugeResetValueAfterNeglect;

            // TODO: Neglect should also decrease Happiness - amount TBD.
            return isTraining ? NeglectCareMistakesWhileTraining : 1;
        }

        return 0;
    }
}
