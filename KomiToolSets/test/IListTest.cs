using KomiToolSets.IListPack;
using NUnit.Framework;

namespace KomiToolSets.test;

[TestFixture]
public class IListTest
{
    [Test]
    public void Invoke()
    {
        var ls = new double[5000];
        for (var i = 0; i < 5000; i++)
        {
            ls[i] = new Random().NextDouble();
        }

        var result = DefaultIEnumerable.AggregateInt32Fast(new ReadOnlySpan<double>(ls));
    }
}