
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface ICandlesLocalStorage
{
    public void Fill(IEnumerable<Candle> candles);

    public bool Any(Guid instrumentId);

    public void Update(Candle candle);

    public Candle[] Get(Guid instrumentId);

    public void Clear();
}
