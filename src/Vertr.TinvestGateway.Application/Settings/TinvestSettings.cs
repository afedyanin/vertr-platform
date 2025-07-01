using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.Application.Settings;
public class TinvestSettings
{
    public InvestApiSettings? InvestApiSettings { get; set; }

    public bool OrderTradesStreamEnabled { get; set; }

    public bool OrderStateStreamEnabled { get; set; }

    public bool MarketDataStreamEnabled { get; set; }

    public bool WaitCandleClose { get; set; }
}
