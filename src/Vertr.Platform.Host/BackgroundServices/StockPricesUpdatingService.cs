using Vertr.Platform.BlazorUI.Components.Models;
using Vertr.Platform.Host.StockTicker;

namespace Vertr.Platform.Host.BackgroundServices;

public class StockPricesUpdatingService : BackgroundService
{
    private readonly IStockTickerDataHandler _dataHandler;

    public StockPricesUpdatingService(IStockTickerDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var changedStock = new StockModel
            {
                Symbol = "AAA",
                DayOpen = 100,
                DayLow = 90,
                DayHigh = 120,
                LastChange = "103",
                Change = 3,
                PercentChange = 0.04,
                UpdatedAt = DateTime.UtcNow,
            };

            await _dataHandler.HandlePriceChange(changedStock);
            await Task.Delay(1000);
        }
    }
}
