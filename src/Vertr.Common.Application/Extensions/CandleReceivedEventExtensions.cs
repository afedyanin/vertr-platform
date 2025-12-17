using System.Text;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Extensions;

public static class CandleReceivedEventExtensions
{
    public static string Dump(this CandleReceivedEvent evt)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Candle=[{evt.Instrument?.Ticker}][{evt.Candle.Dump()}]");
        sb.AppendLine($"Sample=[{evt.PredictionSampleInfo.Dump()}]");
        sb.AppendLine($"Predictions=[{evt.Predictions.Dump()}]");
        sb.AppendLine($"Quote=[{evt.MarketQuote.Dump()}] Threshold={evt.PriceThreshold}");
        sb.AppendLine($"Signals=[{evt.TradingSignals.Dump()}]");
        sb.AppendLine($"Requests=[{evt.OrderRequests.Dump()}]");

        return sb.ToString();
    }

    internal static string Dump(this Candle? candle)
        => candle == null ? string.Empty : $"{candle.TimeUtc:s} O={candle.Open:N} H={candle.High:N} L={candle.Low:N} C={candle.Close:N} V={candle.Volume:N}";

    internal static string Dump(this IEnumerable<Candle> candles)
    {
        var sb = new StringBuilder();

        foreach (var candle in candles)
        {
            sb.AppendLine(candle.Dump());
        }

        return sb.ToString();
    }

    internal static string Dump(this PredictionSampleInfo info)
        => info == default ? string.Empty : $"F={info.From:s} T={info.To:s} C={info.Count} PMean={info.ClosePriceStats.Mean:N} PStdev={info.ClosePriceStats.StdDev:N}";

    internal static string Dump(this Quote? quote)
        => quote == null ? string.Empty : $"B={quote.Value.Bid:N} A={quote.Value.Ask:N}";

    internal static string Dump(this Prediction prediction)
        => $"{prediction.Predictor}:{prediction.Price:N}";

    internal static string Dump(this IEnumerable<Prediction> predictions)
        => string.Join(", ", predictions.Select(p => p.Dump()));

    internal static string Dump(this TradingSignal signal)
        => $"{signal.Predictor}:{signal.Direction}";

    internal static string Dump(this IEnumerable<TradingSignal> signals)
        => string.Join(", ", signals.Select(p => p.Dump()));

    internal static string Dump(this MarketOrderRequest request)
        => $"{request.Predictor}: Q={request.QuantityLots} ID={request.RequestId}";

    internal static string Dump(this IEnumerable<MarketOrderRequest> requests)
        => string.Join(", ", requests.Select(p => p.Dump()));
}
