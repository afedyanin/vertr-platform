using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Extensions;

public static class InstrumentExtensions
{
    public static string GetTicker(this IEnumerable<Instrument> instruments, Guid id)
        => instruments.FirstOrDefault(c => c.Id == id)?.Ticker ?? string.Empty;

    public static decimal? GetLotSize(this IEnumerable<Instrument> instruments, Guid id)
        => instruments.FirstOrDefault(c => c.Id == id)?.LotSize;

    public static Instrument GetById(this IEnumerable<Instrument> instruments, Guid id)
        => instruments.Single(c => c.Id == id);
}
