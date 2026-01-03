using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

internal interface IHistoricCandlesProvider
{
    public IEnumerable<Candle> Get(Guid instrumentId, int skip = 0, int take = 0);

    public Task Load(string csvPath, Guid instrumentId);

    public CandleRangeInfo? GetRange(Guid instrumentId);
}
