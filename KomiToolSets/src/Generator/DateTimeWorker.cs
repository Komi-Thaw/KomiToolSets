using System.Globalization;

namespace KomiToolSets.Generator;

/// <summary>
/// Datetime Helper
/// </summary>
public class DateTimeWorker
{
    public static long UnixSecondEpoch => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static long UnixMillisecondEpoch => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static long UnixTicks => DateTimeOffset.UnixEpoch.Ticks;

    /// <summary>
    /// 计算天数差距
    /// </summary>
    /// <param name="darg1">比对时间参数</param>
    /// <param name="darg2">被比对时间参数</param>
    /// <returns></returns>
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