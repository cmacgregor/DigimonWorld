// The world clock - drives the day/night cycle (and, later, which wild
// Digimon are around, since that's expected to key off ActiveTime same as
// PartnerDigimon.ActiveTime already does).
public class WorldTime
{
    public const int MinutesPerHour = 60;
    public const int HoursPerDay = 24;

    // Hour bands are an even split of the day - adjust if the real
    // design wants uneven bands (e.g. a longer Day than Night).
    public const int MorningStartHour = 6;
    public const int DayStartHour = 12;
    public const int DuskStartHour = 18;

    // Digimon always wake up at the same fixed clock hour, regardless of
    // when they fell asleep - so a sleep session's actual duration has
    // to be derived from "now until this hour," not chosen freely.
    public const int WakeUpHour = 7;

    public int Day { get; set; }
    public int HourOfDay { get; set; }
    public int MinuteOfHour { get; set; }

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

    public void Advance(int minutes)
    {
        MinuteOfHour += minutes;
        HourOfDay += MinuteOfHour / MinutesPerHour;
        MinuteOfHour %= MinutesPerHour;
        Day += HourOfDay / HoursPerDay;
        HourOfDay %= HoursPerDay;
    }

    // Minutes from right now until the next WakeUpHour:00 - always
    // strictly positive, wrapping to the next day if that hour has
    // already passed (or is right now) today.
    public int MinutesUntilWakeUp()
    {
        var minutesNow = HourOfDay * MinutesPerHour + MinuteOfHour;
        var minutesAtWakeUp = WakeUpHour * MinutesPerHour;

        var minutesUntil = minutesAtWakeUp - minutesNow;
        if (minutesUntil <= 0)
        {
            minutesUntil += HoursPerDay * MinutesPerHour;
        }

        return minutesUntil;
    }
}
