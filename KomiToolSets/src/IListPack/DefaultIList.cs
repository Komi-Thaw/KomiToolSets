using Newtonsoft.Json;

namespace KomiToolSets.IListPack;

public static class DefaultIList
{
    /// <summary>
    /// 嵌套IEnumerable泛型转HashSet
    /// </summary>
    /// <param name="source">源输入</param>
    /// <typeparam name="T">源输入类型(泛型)</typeparam>
    /// <exception cref="ArgumentNullException"></exception>
    public static HashSet<dynamic> NestedToHastSet<T>(this T? source)
    {
        if (source is null)
            throw new ArgumentNullException($"{nameof(source)} value is null!");

        var inout = new HashSet<dynamic>();

        LoopBackFunc(source, ref inout);

        return inout;
    }

    /// <summary>
    /// 自调用方法
    /// </summary>
    /// <param name="thing">源输入</param>
    /// <param name="hash">HashSet输出</param>
    /// <typeparam name="T">预判类型</typeparam>
    private static void LoopBackFunc<T>(T thing, ref HashSet<dynamic> hash)
    {
        if (typeof(T).GenericTypeArguments.Any() &&
            typeof(T).GenericTypeArguments.First().IsGenericType)
            foreach (var one in ((thing as IEnumerable<dynamic>)?.ToList() ?? new List<dynamic>()).TakeWhile(one =>
                         one.GetType().IsGenericType))
            {
                LoopBackFunc(one, ref hash);
            }

        if (typeof(T).GenericTypeArguments.First().IsGenericType) return;

        foreach (var moon in (thing as IEnumerable<dynamic>)?.ToArray() ?? Array.Empty<dynamic>())
        {
            hash.Add(moon);
        }
    }

    /// <summary>
    /// 多项去重
    /// </summary>
    /// <param name="source">源输入</param>
    /// <typeparam name="T">源输入泛型</typeparam>
    public static IEnumerable<T> ToDistinctEnumerable<T>(this IEnumerable<T> source) where T : notnull
    {
        return source.Select(x => JsonConvert.ToString(x)).Distinct()
            .Select(value => JsonConvert.DeserializeObject<T>(value)!);
    }
}