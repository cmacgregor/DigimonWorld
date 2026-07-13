// Gauge is incremented externally whenever the Digimon poops on the
// ground; NeedsToPotty's "full" threshold decision stays external too.
public class PottySystem
{
    public int Gauge { get; set; }
    public bool NeedsToPotty { get; set; }
}
