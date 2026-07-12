// Sleepy isn't flipped by elapsed time here; that threshold decision
// stays external, same as Potty's "full" condition already does.
public class SleepSystem
{
    public int Gauge { get; set; }
    public bool Sleepy { get; set; }

    // Gauge's rate is still a 1:1 placeholder (1 point per hour) until
    // real pacing is designed.
    public void Advance(int wholeHours)
    {
        Gauge += wholeHours;
    }
}
