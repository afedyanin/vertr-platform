using Vertr.Infrastructure.Common.Awaiting;

namespace Vertr.Infrastructure.Common.Tests.Awating;

[TestFixture(Category = "Unit")]
public class AwatingServiceTests
{
    [Test]
    public async Task CanCompleteCommand()
    {
        var service = new AwatingService<int>();
        using var cts = new CancellationTokenSource();

        var results = new string[]
        {
            ""
        };

        _ = Task.Run(async () =>
        {
            await Task.Delay(100);
            results[0] = "Completed";
            service.SetCompleted(0);
        });

        _ = await service.WaitToComplete(0, cts.Token);

        Assert.That(results[0], Is.EqualTo("Completed"));

        Console.WriteLine(service.GetStatistics());
    }

    [Test]
    public async Task CanCancelCommand()
    {
        var service = new AwatingService<int>();
        using var cts = new CancellationTokenSource();

        var results = new string[]
        {
            ""
        };

        _ = Task.Run(async () =>
        {
            await Task.Delay(100);
            cts.Cancel();
        });

        try
        {
            _ = await service.WaitToComplete(0, cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        Assert.That(results[0], Is.EqualTo(""));
        Console.WriteLine(service.GetStatistics());
    }

    [Test]
    public async Task CanCancelCommandByTimeout()
    {
        var service = new AwatingService<int>();
        using var cts = new CancellationTokenSource(37);

        var results = new string[]
        {
            ""
        };

        _ = Task.Run(async () =>
        {
            await Task.Delay(100);
            results[0] = "Completed";
            service.SetCompleted(0);
        });

        try
        {
            _ = await service.WaitToComplete(0, cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        Assert.That(results[0], Is.EqualTo(""));
        Console.WriteLine(service.GetStatistics());
    }

    [Test]
    public async Task CanSetCompletedBeforeAwait()
    {
        var service = new AwatingService<int>();
        using var cts = new CancellationTokenSource(15000);

        var results = new string[]
        {
            ""
        };

        results[0] = "Completed";
        service.SetCompleted(0);

        try
        {
            _ = await service.WaitToComplete(0, cts.Token);
        }
        catch (TaskCanceledException)
        {
        }

        Assert.That(results[0], Is.EqualTo("Completed"));
        Console.WriteLine(service.GetStatistics());
    }

    [Test]
    public async Task CaтAwaitTwice()
    {
        var service = new AwatingService<int>();
        using var cts = new CancellationTokenSource(5000);

        var results = new string[]
        {
            ""
        };

        _ = Task.Run(async () =>
        {
            await Task.Delay(100);
            results[0] = "Completed";
            service.SetCompleted(0);
        });

        try
        {
            var t1 = service.WaitToComplete(0, cts.Token);
            var t2 = service.WaitToComplete(0, cts.Token);
            await Task.WhenAll(t1, t2);
        }
        catch (TaskCanceledException)
        {
        }

        Assert.That(results[0], Is.EqualTo("Completed"));
        Console.WriteLine(service.GetStatistics());
    }
}
