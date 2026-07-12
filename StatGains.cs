// A bundle of per-stat deltas, all defaulting to 0. Training, battle, and
// food each produce one of these (however each source computes its own
// amounts) and hand it to PartnerDigimon.ApplyStatGains - one shared
// application point regardless of where the gain came from.
public class StatGains
{
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public int Brains { get; set; }
    public int MaxHP { get; set; }
    public int MaxMP { get; set; }
}
