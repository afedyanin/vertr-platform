using Microsoft.Extensions.Time.Testing;

namespace Vertr.Experimental.Tests;

public class TimeProviderTests
{
    [Test]
    public void CanGetCurrentTime()
    {
        var timeProvider = TimeProvider.System;
        var now = timeProvider.GetUtcNow();
        Console.WriteLine(now);
        Assert.Pass();
    }

    [Test]
    public void CanUseFakeTime()
    {
        var fake = new FakeTimeProvider();
        fake.SetUtcNow(new DateTime(2025, 02, 27));
        var step = TimeSpan.FromHours(1);

        var now = fake.GetUtcNow();
        Console.WriteLine(now);

        for (int i = 1; i <= 10; i++)
        {
            fake.Advance(step);
            now = fake.GetUtcNow();
            Console.WriteLine(now);
        }

        Assert.Pass();
    }
}
