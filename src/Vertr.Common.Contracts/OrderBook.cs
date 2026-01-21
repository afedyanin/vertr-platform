using System.Text;
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

    public override string? ToString()
    {
        var sb = new StringBuilder();

        sb.Append($"UpdatedAt={UpdatedAt:O} MidPrice={MidPrice} MaxBid={MaxBid} MinAsk={MinAsk} Depth={Depth} ");
        sb.Append($"BidsCount={Bids.Sum(b => b.QtyLots)} AskCount={Asks.Sum(b => b.QtyLots)} ");
        sb.Append($"BidsValue={Bids.Sum(b => b.QtyLots * b.Price)} AskValue={Asks.Sum(b => b.QtyLots * b.Price)} ");

        return sb.ToString();
    }
}
