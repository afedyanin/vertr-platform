using Vertr.Common.Application.Indicators;

namespace Vertr.Common.Aplication.Tests.Indicators;

public class BasicStatsGaugeTests
{
    [Test]
    public void CanAddNewValue()
    {
        var gauge = new BasicStatsGauge();

        var value = 156;
        gauge.Add(value);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(gauge.Count, Is.EqualTo(1));
            Assert.That(gauge.Average, Is.EqualTo(value));
            Assert.That(gauge.Last, Is.EqualTo(value));
            Assert.That(gauge.StdDev, Is.EqualTo(0));
        }
    }

    [Test]
    public void CanAddNewValues()
    {
        var gauge = new BasicStatsGauge();

        gauge.Add(2);
        gauge.Add(4);

        Assert.Multiple(() =>
        {
            Assert.That(gauge.Count, Is.EqualTo(2));
            Assert.That(gauge.Last, Is.EqualTo(4));
            Assert.That(gauge.Average, Is.EqualTo(3));
            Assert.That(Math.Round(gauge.StdDev, 4), Is.EqualTo(1.4142));
        });
    }
}
