using Vertr.MarketData.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public record SubscriptionModel
{
    public required CandleSubscription Subscription { get; init; }

    public required string InstrumentName { get; init; }
}
