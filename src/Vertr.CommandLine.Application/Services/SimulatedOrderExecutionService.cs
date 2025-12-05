using System.Diagnostics;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Abstracttions;

namespace Vertr.CommandLine.Application.Services;

internal class SimulatedOrderExecutionService : IOrderExecutionService
{
    private readonly IMarketDataService _marketDataService;

    public SimulatedOrderExecutionService(IMarketDataService marketDataService)
    {
        _marketDataService = marketDataService;
    }

    public Task<Trade[]> PostOrder(
        string symbol,
        decimal qty,
        decimal price,
        decimal comissionPercent)
    {
        // For simulated execution marketTime must be set.
        Debug.Assert(price != decimal.Zero);
        Debug.Assert(qty != decimal.Zero);

        var trade = new Trade
        {
            Price = price,
            Quantity = qty,
            Comission = Math.Abs(price * qty * comissionPercent),
        };

        return Task.FromResult<Trade[]>([trade]);
    }
}