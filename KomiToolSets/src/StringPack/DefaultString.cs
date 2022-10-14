namespace KomiToolSets.StringPack;

public static class DefaultString
{
    /// <summary>
    /// 字符串去空白
    /// </summary>
    /// <param name="source">字符串输入</param>
    /// <param name="opt">TrimOptions枚举</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ToTrimmedString(this string? source, trimOptions opt = trimOptions.Both)
    {
        if (string.IsNullOrEmpty(source))
            throw new ArgumentNullException($"{nameof(source)} value is null!");

        if (opt is trimOptions.Full) return source.Replace(" ", string.Empty);

        var str = opt switch
        {
            trimOptions.Left => source.TrimStart(),
            trimOptions.Right => source.TrimEnd(),
            trimOptions.Both => source.Trim(),
            _ => string.Empty
        };

        return str;
    }

    /// <summary>
    /// 两块字符串比对
    /// </summary>
    /// <param name="block1">比对对象</param>
    /// <param name="block2">被比对对象</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static (bool isIdentical, string lines) DiffWithEveryLine(string block1, string block2)
    {
        if (string.IsNullOrEmpty(block1) || string.IsNullOrEmpty(block2))
            throw new ArgumentNullException(
                $"one of the ({nameof(block1)} or {nameof(block2)}) parameter is null value!");

        var block1Vals = block1.Split("\r\n");
        var block2Vals = block2.Split("\r\n");

        if (!block1.Equals(block2) || !block1Vals.Length.Equals(block2Vals.Length)) return (default, default)!;

        var lineNums = new int[block1Vals.Length];
        int lineNum = default;
        var fau = true;

        foreach (var selLine in block1Vals)
        {
            fau |= selLine.Equals(block2Vals[lineNum]);
            if (!fau) lineNums[lineNum] = lineNum;
            lineNum += 1;
        }

        var res = !fau ? lineNums.Select(x => x.ToString()).Aggregate((s, s1) => s + "," + s1) : default;
        return (fau, res)!;
    }
}

public enum trimOptions
{
    Full,
    Left,
    Right,
    Both
}