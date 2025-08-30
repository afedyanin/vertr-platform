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
    }

    /* TODO: Implemnt Tests:
     Cancel Task
     Cancel Timeout
     SetCompleted before Wait
     Cuncurrency Check
     Dump internal state
     */
}
