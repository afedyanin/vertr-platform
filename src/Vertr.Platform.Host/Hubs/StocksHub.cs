using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Host.StockTicker;
using Vertr.Platform.Host.Extensions;

namespace Vertr.Platform.Host.Hubs;

public class StocksHub : Hub
{
    private readonly IStockTickerObservable _stockTickerObservable;

    public StocksHub(IStockTickerObservable stockTickerObservable)
    {
        _stockTickerObservable = stockTickerObservable;
    }

    public Task<StockModel[]> GetSnapshot()
    {
        var res = new List<StockModel>();

        res.Add(new StockModel
        {
            Symbol = "AAA",
            DayOpen = 100,
            DayLow = 90,
            DayHigh = 120,
            LastChange = 103,
            Change = 3,
            PercentChange = 0.04,
            UpdatedAt = DateTime.UtcNow,
        });

        res.Add(new StockModel
        {
            Symbol = "BBB",
            DayOpen = 100,
            DayLow = 90,
            DayHigh = 120,
            LastChange = 103,
            Change = 3,
            PercentChange = 0.04,
            UpdatedAt = DateTime.UtcNow,
        });

        res.Add(new StockModel
        {
            Symbol = "CCC",
            DayOpen = 100,
            DayLow = 90,
            DayHigh = 120,
            LastChange = 103,
            Change = 3,
            PercentChange = 0.04,
            UpdatedAt = DateTime.UtcNow,
        });

        return Task.FromResult(res.ToArray());
    }

    public ChannelReader<StockModel> StreamStocks()
    {
        return _stockTickerObservable.StreamStocks().AsChannelReader(10);
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}
