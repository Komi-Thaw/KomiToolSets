using KomiToolSets.StringPack;
using NUnit.Framework;

namespace KomiToolSets.test;

/// <summary>
/// 基准测试
/// </summary>
[TestFixture]
public class TestEntrance
{
    private readonly string name = "jacobs mingw weller";

    [Test]
    public void CharContainsTest()
    {
        Assert.IsFalse(name.CharContains('a'));
    }
}