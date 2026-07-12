using System;

// Energy is separate from Hunger - it's a second countdown that only
// starts draining once Hungry, and once it bottoms out, starvation
// weight loss kicks in. Task/battle/training-driven drain isn't modeled
// yet, same as the stat-gain formulas those would feed.
public class EnergySystem
{
    public const int GaugeMax = 240; // placeholder
    public const int StarvationWeightLossIntervalMinutes = 10;
    public const int StarvationWeightLossGrams = 1;
    public const int SleepStarvationWeightLossGramsPerHour = 1;

    public int Gauge { get; set; }
    // Accumulates minutes spent starving (Hungry with Gauge depleted)
    // between StarvationWeightLossIntervalMinutes ticks, so weight loss
    // is correct regardless of how the caller chunks up Advance calls.
    public int StarvationMinuteAccumulator { get; set; }

    // Drains while hungry, and returns grams of weight lost to
    // starvation this tick (0 if none) - Weight itself lives on
    // PartnerDigimon. isSleeping switches to the flat per-hour sleep
    // rate (Energy can't replenish without eating either way) instead
    // of the normal 10-minute-interval tick.
    public int Advance(int minutes, int wholeHours, bool hungry, bool isSleeping)
    {
        if (hungry)
        {
            Gauge = Math.Max(0, Gauge - minutes);
        }

        if (isSleeping)
        {
            return hungry ? wholeHours * SleepStarvationWeightLossGramsPerHour : 0;
        }

        if (Gauge <= 0 && hungry)
        {
            StarvationMinuteAccumulator += minutes;
            var gramsLost = (StarvationMinuteAccumulator / StarvationWeightLossIntervalMinutes) * StarvationWeightLossGrams;
            StarvationMinuteAccumulator %= StarvationWeightLossIntervalMinutes;
            return gramsLost;
        }

        StarvationMinuteAccumulator = 0;
        return 0;
    }
}
