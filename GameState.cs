public class GameState
{
    public PartnerDigimon CurrentPartner { get; set; }
    public int Bits { get; set; }
    public WorldTime CurrentTime { get; } = new();

    // Advances the world clock and the active partner's time-driven care
    // gauges together, since they're driven by the same passage of time.
    public void AdvanceTime(int minutes, bool isTraining = false)
    {
        CurrentTime.Advance(minutes);
        CurrentPartner?.AdvanceTime(minutes, isTraining);
    }
}
