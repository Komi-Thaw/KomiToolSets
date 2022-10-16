using System.Globalization;

namespace KomiToolSets.Generator;

public class DateTimeWorker
{
    public static long UnixSecondEpoch => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static long UnixMillisecondEpoch => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static long UnixTicks => DateTimeOffset.UnixEpoch.Ticks;

    public static int DaysElapsed(DateTime darg1, DateTime darg2)
    {
        TimeSpan result;
        try
        {
            result = new TimeSpan(DateTimeOffset.Parse(darg1.ToString(CultureInfo.InvariantCulture)).UtcTicks) -
                     new TimeSpan(DateTimeOffset.Parse(darg2.ToString(CultureInfo.InvariantCulture)).UtcTicks);
        }
        catch (Exception)
        {
            return default;
        }

        return Convert.ToInt32(result.TotalDays);
    }
}