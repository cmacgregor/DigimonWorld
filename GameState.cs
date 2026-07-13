public class GameState
{
    public PartnerDigimon CurrentPartner { get; set; }
    public int Bits { get; set; }
    public WorldTime CurrentTime { get; } = new();
    public Inventory Inventory { get; } = new();

    // Advances the world clock and the active partner's time-driven care
    // gauges together, since they're driven by the same passage of time.
    public void AdvanceTime(int minutes)
    {
        CurrentTime.Advance(minutes);
        CurrentPartner?.AdvanceTime(minutes);
    }

    // The player's sleep command, enabled once the partner is Sleepy.
    // Digimon always wake up at the same fixed clock hour, so the sleep
    // duration isn't up to the caller - it's however long remains until
    // WorldTime.WakeUpHour from right now.
    public void Sleep()
    {
        var minutes = CurrentTime.MinutesUntilWakeUp();
        CurrentTime.Advance(minutes);
        CurrentPartner?.AdvanceTimeAsleep(minutes);
    }
}
