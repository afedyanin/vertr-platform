using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.Application.Internal;

internal class MarketDataManager : IMarketDataManager
{
    public Task<CandleInstrument[]> GetCandleSubscriptions()
        => Task.FromResult(new CandleInstrument[]
        {
            // GAZP
            new CandleInstrument("962e2a95-02a9-4171-abd7-aa198dbe643a", CandleInterval.Min_1),
            // LKOH
            new CandleInstrument("02cfdf61-6298-4c0f-a9ca-9cabc82afaf3", CandleInterval.Min_1),
            // OZON
            new CandleInstrument("35fb8d6b-ed5f-45ca-b383-c4e3752c9a8a", CandleInterval.Min_1),
            // SBER
            new CandleInstrument("e6123145-9665-43e0-8413-cd61b8aa9b13", CandleInterval.Min_1),
            // T
            new CandleInstrument("87db07bc-0e02-4e29-90bb-05e8ef791d7b", CandleInterval.Min_1),
        });
}
