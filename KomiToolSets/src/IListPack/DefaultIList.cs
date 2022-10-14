namespace KomiToolSets.IListPack;

public static class DefaultIList
{
    public static HashSet<dynamic> NestedToHastSet<T>(this T? source)
    {
        if (source is null)
            throw new ArgumentNullException($"{nameof(source)} value is null!");

        var inout = new HashSet<dynamic>();

        LoopBackFunc(source, ref inout);

        return inout;
    }

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
}