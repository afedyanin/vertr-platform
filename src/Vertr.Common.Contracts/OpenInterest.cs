using System.Text.Json;

namespace Vertr.Common.Contracts;

public record class OpenInterest
{
    public DateTime Time { get; set; }

    public Guid InstrumentId { get; set; }

    // Открытый интерес (Open Interest, OI) во фьючерсах — это общее количество активных, еще не закрытых фьючерсных контрактов на рынке,
    // показывающее, сколько участников держат позиции, в отличие от объема торгов, который показывает количество сделок.
    public long Quantity { get; set; }

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static OpenInterest? FromJson(string json) => JsonSerializer.Deserialize<OpenInterest>(json, JsonOptions.DefaultOptions);
}
