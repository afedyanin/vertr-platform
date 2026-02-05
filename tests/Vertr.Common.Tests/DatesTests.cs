namespace Vertr.Common.Tests;

public class DatesTests
{
    [Test]
    public void CanCompareDates()
    {
        var d1 = new DateOnly(2023, 12, 31);
        var d2 = new DateOnly(2023, 1, 1);

        var totalDays = d1.DayNumber - d2.DayNumber; // Result: 364

        Assert.That(totalDays, Is.EqualTo(364));
    }
}
