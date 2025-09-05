using Vertr.Platform.BlazorUI.Components.Models;

namespace Vertr.Platform.Host.StockTicker;

public interface IStockTickerDataHandler
{
    public Task HandlePriceChange(StockModel stock);
}
