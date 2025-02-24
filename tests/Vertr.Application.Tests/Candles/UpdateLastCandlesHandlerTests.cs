using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Application.Candles;
using Vertr.Domain.Enums;

namespace Vertr.Application.Tests.Candles;

[TestFixture(Category = "System", Explicit = true)]
public class UpdateLastCandlesHandlerTests : ApplicationTestBase
{
    private const string _symbol = "SBER";
    private const CandleInterval _interval = CandleInterval._10Min;

    [Test]
    public async Task CanGetLoadingInterval()
    {
        var handler = ServiceProvider.GetRequiredService<UpdateLastCandlesHandler>();

        (var from, var to) = await handler.GetLoadingInterval(_symbol, _interval);

        Assert.Multiple(() =>
        {
            Assert.That(from, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(to, Is.Not.EqualTo(DateTime.MinValue));
        });

        Console.WriteLine($"Loading interval: from={from:O} to={to:O}");
    }

    [Test]
    public async Task CanLoadSymbol()
    {
        var dbCandles = await Repo.GetLast(_symbol, _interval, count: 1, completedOnly: true);
        var lastTimeBefore = dbCandles.Select(c => c.TimeUtc).FirstOrDefault();

        var handler = ServiceProvider.GetRequiredService<UpdateLastCandlesHandler>();
        await handler.LoadSymbol(_symbol, _interval, CancellationToken.None);

        dbCandles = await Repo.GetLast(_symbol, _interval, count: 1, completedOnly: true);
        var lastTimeAfter = dbCandles.Select(c => c.TimeUtc).FirstOrDefault();

        Console.WriteLine($"Last candle time: before={lastTimeBefore:O} after={lastTimeAfter:O}");
        Assert.That(lastTimeBefore, Is.LessThanOrEqualTo(lastTimeAfter));
    }

    [Test]
    public async Task CanSendUpdateRequestWithSingleSymbol()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();

        var request = new UpdateLastCandlesRequest
        {
            Symbols = [_symbol],
            Interval = _interval,
        };

        await mediator.Send(request);

        Assert.Pass();
    }

    [Test]
    public async Task CanSendUpdateRequestWithManySymbols()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();

        var request = new UpdateLastCandlesRequest
        {
            Symbols = ["AFKS", "MOEX", "OZON"],
            Interval = _interval,
        };

        await mediator.Send(request);

        Assert.Pass();
    }
}
