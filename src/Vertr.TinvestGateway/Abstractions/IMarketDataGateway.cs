using Vertr.Common.Contracts;
using Vertr.TinvestGateway.Models;

namespace Vertr.TinvestGateway.Abstractions;

public interface IMarketDataGateway
{
    public Task<Instrument[]?> FindInstrument(string query);

    public Task<Instrument?> GetInstrumentBySymbol(string classCode, string ticker);

    public Task<Instrument?> GetInstrumentById(Guid instrumentId);

    public Task<Candlestick[]> GetCandles(
        Guid instrumentId,
        DateOnly? date = null,
        CandleInterval interval = CandleInterval.Min_1);
}