using Vertr.Common.Contracts;

namespace Vertr.Common.Aplication.Tests;

public class TradingDiretionTests
{
    [TestCase(110, TradingDirection.Hold)]
    [TestCase(90, TradingDirection.Sell)]
    [TestCase(130, TradingDirection.Buy)]
    [TestCase(97, TradingDirection.Hold)]
    [TestCase(128, TradingDirection.Hold)]
    public void CanGetTradingDirection(decimal fairPrice, TradingDirection expectedDirection)
    {
        var threshold = 0.08; // percent
        var quote = new Quote
        {
            Ask = 120.0m,
            Bid = 100.0m,
            Time = DateTime.UtcNow
        };

        var direction = GetTradingDirection(fairPrice, quote, threshold);

        Assert.That(direction, Is.EqualTo(expectedDirection));
    }

    internal static TradingDirection GetTradingDirection(decimal fairPrice, Quote marketQuote, double threshold)
    {
        // цена будет выше минимальной цены предложения
        var askDelta = (double)((fairPrice - marketQuote.Ask) / marketQuote.Ask);
        if (askDelta >= threshold)
        {

            return TradingDirection.Buy;
        }

        // цена будет ниже максимальной цены спроса
        var bidDelta = (double)((marketQuote.Bid - fairPrice) / fairPrice);
        return bidDelta >= threshold ? TradingDirection.Sell : TradingDirection.Hold;
    }
}
