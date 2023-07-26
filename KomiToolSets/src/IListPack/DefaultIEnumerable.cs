using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace KomiToolSets.IListPack;

/// <summary>
/// IEnumerable Helper
/// </summary>
public static class DefaultIEnumerable
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
    public static IEnumerable<T?> ToDistinctEnumerable<T>(this IEnumerable<T> source)
        where T : notnull
    {
        return source.Select(x => JsonConvert.ToString(x)).Distinct()
            .Select(JsonConvert.DeserializeObject<T>);
    }

    /// <summary>
    /// 快速遍历
    /// </summary>
    /// <param name="source">源输入</param>
    /// <typeparam name="T">源输入泛型</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> LazyReturnValue<T>(this IEnumerable<T> source)
        where T : notnull
    {
        var res = new ReadOnlyMemory<T>(source.ToArray());

        foreach(var ind in MemoryMarshal.ToEnumerable<T>(res))
        {
            yield return ind;
        }
    }

    public static int AggregateInt32Fast(ReadOnlySpan<double> source)
    {
        var arg = new ReadOnlySpan<int>(source.ToArray().Select(Convert.ToInt32).ToArray());
        return AggregateInt32Fast(arg);
    }

    /// <summary>
    /// 数值组合计算
    /// </summary>
    /// <param name="source">源输入</param>
    /// <returns>总和</returns>
    public static int AggregateInt32Fast(ReadOnlySpan<int> source)
    {
        var sum = 0;
        if (Vector.IsHardwareAccelerated && source.Length > Vector<int>.Count * 2) // use SIMD
        {
            var vectors = MemoryMarshal.Cast<int, Vector<int>>(source);
            var vectorSum = Vector<int>.Zero;

            foreach (var vector in vectors)
                vectorSum += vector;

            for (var index = 0; index < Vector<int>.Count; index++)
                sum += vectorSum[index];

            var count = source.Length % Vector<int>.Count;
            source = source.Slice(source.Length - count, count);
        }

        foreach (var item in source)
            sum += item;

        return sum;
    }
}