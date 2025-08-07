namespace Vertr.Experimental.Tests;
public class InterlockedTests
{
    private int _counter = 0;

    [Test]
    public async Task CanUseCompareExchange()
    {
        var cts = new CancellationTokenSource(5000);
        var t1 = DoWorkSet(cts.Token);
        var t2 = DoWorkReset(cts.Token);
        await Task.WhenAll(t1, t2);

        Console.WriteLine($"Last Value = {_counter}");
    }

    private async Task DoWorkSet(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var oldValue = Interlocked.CompareExchange(ref _counter, 1, 0);
            Console.WriteLine($"DoWorkSet oldValue={oldValue} currentValue={_counter}");
            await Task.Delay(87);
        }
    }

    private async Task DoWorkReset(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var oldValue = Interlocked.CompareExchange(ref _counter, 0, 1);
            Console.WriteLine($"DoWorkSet oldValue={oldValue} currentValue={_counter}");
            await Task.Delay(117);
        }
    }
}
