using Vertr.Platform.BlazorUI.Components.Models;

namespace Vertr.Platform.Host.StockTicker;

public interface IStockTickerObservable
{
    public IObservable<StockModel> StreamStocks();
}
