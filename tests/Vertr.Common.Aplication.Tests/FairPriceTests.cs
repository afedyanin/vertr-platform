namespace Vertr.Common.Aplication.Tests;

public class FairPriceTests
{
    [Test]
    public void CanGetFairPrice()
    {
        var rusfar = 15.05m;
        var daysToExpiry = 196;
        var spot = 314.455m;

        var expected = 314.455m;

        var fairPrice = GetFairPrice(spot, rusfar, daysToExpiry);

        Console.WriteLine($"fairPrice={fairPrice}");
        Assert.That(fairPrice, Is.EqualTo(expected));
    }

    private static decimal GetFairPrice(decimal spotPrice, decimal rate, int daysToExpiry)
        => spotPrice * (1 + (rate / 100.0m) * (daysToExpiry / 365.0m));


}
