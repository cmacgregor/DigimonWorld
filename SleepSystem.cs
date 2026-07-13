// Sleepy isn't flipped by elapsed time here; that threshold decision
// stays external, same as Potty's "full" condition already does.
public class SleepSystem
{
    public int Gauge { get; set; }
    public bool Sleepy { get; set; }

    // TODO: Placeholder rate (1 point per hour, 1:1) - replace once real
    // pacing is designed.
    public void Advance(int wholeHours)
    {
        Gauge += wholeHours;
    }
}
