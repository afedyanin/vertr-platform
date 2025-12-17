using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Extensions;

public static class InstrumentExtensions
{
    public static string GetTicker(this IEnumerable<Instrument> instruments, Guid id)
        => instruments.FirstOrDefault(c => c.Id == id)?.Ticker ?? string.Empty;
}
