using Tinkoff.InvestApi;
using Vertr.Common.Contracts;

namespace Vertr.TinvestGateway;

public class TinvestSettings
{
    public InvestApiSettings? InvestApiSettings { get; set; }

    public bool OrderTradesStreamEnabled { get; set; }

    public bool OrderStateStreamEnabled { get; set; }

    public bool MarketDataStreamEnabled { get; set; }

    public bool WaitCandleClose { get; set; }

    public string AccountId { get; set; } = string.Empty;

    public SubscriptionRequest[] Subscriptions { get; set; } = [];

    public Dictionary<string, Guid> Currencies { get; set; } = [];
}

public record class SubscriptionRequest
{
    public Guid InstrumentId { get; set; }

    public CandleInterval Interval { get; set; }

    public bool Disabled { get; set; }

    public int MaxCount { get; set; }

    public int OrderBookDepth { get; set; }
}