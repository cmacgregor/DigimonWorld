// The world clock - drives the day/night cycle (and, later, which wild
// Digimon are around, since that's expected to key off ActiveTime same as
// PartnerDigimon.ActiveTime already does).
public class WorldTime
{
    public const int HoursPerDay = 24;

    // Hour bands are an even split of the day - adjust if the real
    // design wants uneven bands (e.g. a longer Day than Night).
    public const int MorningStartHour = 6;
    public const int DayStartHour = 12;
    public const int DuskStartHour = 18;

    public int Day { get; set; }
    public int HourOfDay { get; set; }

    public ActiveTimeEnum ActiveTime
    {
        get
        {
            if (HourOfDay >= DuskStartHour) return ActiveTimeEnum.Dusk;
            if (HourOfDay >= DayStartHour) return ActiveTimeEnum.Day;
            if (HourOfDay >= MorningStartHour) return ActiveTimeEnum.Morning;
            return ActiveTimeEnum.Night;
        }
    }

    public void Advance(int hours)
    {
        HourOfDay += hours;
        Day += HourOfDay / HoursPerDay;
        HourOfDay %= HoursPerDay;
    }
}
