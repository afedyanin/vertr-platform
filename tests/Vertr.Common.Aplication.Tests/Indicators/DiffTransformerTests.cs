using Vertr.Strategies.FuturesArbitrage.Indicators;

namespace Vertr.Common.Aplication.Tests.Indicators;

public class DiffTransformerTests
{
    [Test]
    public void InitialValueDiffIsNull()
    {
        var dt = new DiffTransformer();
        var diff = dt.Diff(10);
        Assert.That(diff, Is.Null);
    }

    [Test]
    public void CanGetDiff()
    {
        var dt = new DiffTransformer(2);
        var diff = dt.Diff(4);
        Assert.That(diff, Is.EqualTo(1));
    }

    [Test]
    public void CanGetDiffSqeuentially()
    {
        var dt = new DiffTransformer(0);

        Assert.That(dt.Diff(2), Is.EqualTo(2));
        Assert.That(dt.Diff(4), Is.EqualTo(1));
        Assert.That(dt.Diff(6), Is.EqualTo((6 - 4) / 4.0));
        Assert.That(dt.Diff(12), Is.EqualTo((12 - 6) / 6.0));
        Assert.That(dt.Diff(20), Is.EqualTo((20 - 12) / 12.0));
    }
}
