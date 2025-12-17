
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface ICandlesLocalStorage
{
    public int CandlesBufferLength { get; set; }

    public void Load(IEnumerable<Candle> candles);

    public void Update(Candle candle, bool recalculateStats = true);

    public Candle[] Get(Guid instrumentId);

    public int GetCount(Guid instrumentId);
}
