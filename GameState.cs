public class GameState
{
    public PartnerDigimon CurrentPartner { get; set; }
    public int Bits { get; set; }
    public WorldTime CurrentTime { get; } = new();

    // Advances the world clock and the active partner's time-driven care
    // gauges together, since they're driven by the same passage of time.
    public void AdvanceTime(int minutes)
    {
        CurrentTime.Advance(minutes);
        CurrentPartner?.AdvanceTime(minutes);
    }

    // The player's sleep command, enabled once the partner is Sleepy.
    // Advances the same world clock, but tells the partner it's asleep
    // for the duration instead of ticking normally.
    public void Sleep(int minutes)
    {
        CurrentTime.Advance(minutes);
        CurrentPartner?.AdvanceTimeAsleep(minutes);
    }
}
