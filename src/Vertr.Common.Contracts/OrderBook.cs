using System.Text.Json;

namespace Vertr.Common.Contracts;

public record class OrderBook
{
    public Guid InstrumentId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsConsistent { get; set; }

    public int Depth { get; set; }

    public Order[] Bids { get; set; } = [];

    public Order[] Asks { get; set; } = [];

    public decimal MaxBid => Bids.Max(b => b.Price);
    public decimal MinAsk => Asks.Min(b => b.Price);

    public decimal MidPrice => (MaxBid + MinAsk) / 2;

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static OrderBook? FromJson(string json) => JsonSerializer.Deserialize<OrderBook>(json, JsonOptions.DefaultOptions);
}
