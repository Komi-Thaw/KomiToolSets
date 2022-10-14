using KomiToolSets.IListPack;
using NUnit.Framework;

namespace KomiToolSets.test;

[TestFixture]
public class IListTest
{
    [Test]
    public void Invoke()
    {
        IEnumerable<IEnumerable<IEnumerable<string>>> a = new List<List<List<string>>>
        {
            new() { new() { "xim", "xim", "hash" }, new() { "xim", "xim", "jinx" } },
            new() { new() { "zack", "riders" } }
        };
        var data = a.NestedToHastSet();
    }
}