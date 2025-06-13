namespace Vertr.MarketData.Contracts;

public record class CandleSubscriptionRequest(
    string Symbol,
    CandleInterval CandleInterval,
    bool WaitingClose = false);
