using Vertr.MarketData.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Models;

public record SubscriptionModel
{
    public required CandleSubscription Subscription { get; init; }

    public required Instrument Instrument { get; set; }
}
