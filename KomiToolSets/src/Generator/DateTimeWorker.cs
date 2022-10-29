using System.Globalization;

namespace KomiToolSets.Generator;

/// <summary>
/// Datetime Helper
/// </summary>
public static class DateTimeWorker
{
    public static long UnixSecondEpoch => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static long UnixMillisecondEpoch => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static long UnixTicks => DateTimeOffset.UnixEpoch.Ticks;

    /// <summary>
    /// 计算天数差距
    /// </summary>
    /// <param name="darg1">比对时间参数</param>
    /// <param name="darg2">被比对时间参数</param>
    public static int DaysElapsed(DateTime darg1, DateTime darg2)
    {
        int result;
        try
        {
            var check = new TimeSpan(DateTimeOffset.Parse(darg1.ToString(CultureInfo.InvariantCulture)).UtcTicks) -
                        new TimeSpan(DateTimeOffset.Parse(darg2.ToString(CultureInfo.InvariantCulture)).UtcTicks);
            result = Convert.ToInt32(Math.Abs(check.TotalDays));
        }
        catch (Exception)
        {
            return -1;
        }

        return result;
    }

    /// <summary>
    /// 计算毫秒差距
    /// </summary>
    /// <param name="darg1">比对时间</param>
    /// <param name="darg2">被比对时间</param>
    public static long MillisecondsElapsed(DateTime darg1, DateTime darg2)
    {
        long result;
        try
        {
            var check = new TimeSpan(DateTimeOffset.Parse(darg1.ToString(CultureInfo.InvariantCulture)).UtcTicks) -
                        new TimeSpan(DateTimeOffset.Parse(darg2.ToString(CultureInfo.InvariantCulture)).UtcTicks);
            result = Convert.ToInt64(Math.Abs(check.TotalMilliseconds));
        }
        catch (Exception)
        {
            return -1;
        }

        return result;
    }
}