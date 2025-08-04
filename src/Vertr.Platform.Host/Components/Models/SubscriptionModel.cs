using Vertr.MarketData.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public record SubscriptionModel
{
    public required CandleSubscription Subscription { get; init; }

    public string InstrumentName => $"{Instrument.Symbol.ClassCode}.{Instrument.Symbol.Ticker} ({Instrument.Name})";

    public required Instrument Instrument { get; set; }
}
