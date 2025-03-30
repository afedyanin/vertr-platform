using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vertr.Terminal.Shared.Models;

namespace Vertr.Terminal.Shared.Hubs;

public class StockTickerHub : Hub
{
    private readonly StockTicker _stockTicker;

    public StockTickerHub(StockTicker stockTicker)
    {
        _stockTicker = stockTicker;
    }

    public IEnumerable<Stock> GetAllStocks()
    {
        return _stockTicker.GetAllStocks();
    }

    public ChannelReader<Stock> StreamStocks()
    {
        return _stockTicker.StreamStocks().AsChannelReader(10);
    }

    public string GetMarketState()
    {
        return _stockTicker.MarketState.ToString();
    }

    public async Task OpenMarket()
    {
        await _stockTicker.OpenMarket();
    }

    public async Task CloseMarket()
    {
        await _stockTicker.CloseMarket();
    }

    public async Task Reset()
    {
        await _stockTicker.Reset();
    }
}
