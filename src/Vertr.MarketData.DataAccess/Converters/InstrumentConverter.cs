using System.Text.Json;
using StackExchange.Redis;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Converters;
internal static class InstrumentConverter
{
    public static Instrument? Convert(this RedisValue source)
        => JsonSerializer.Deserialize<Instrument>(source!);

    public static Instrument[]? Convert(this HashEntry[] source)
        => [.. source.Select(e => e.Value.Convert())];
}
