using System.Text.Json;

namespace Vertr.TinvestGateway.Contracts.Portfolios;

public record class Portfolio
{
    public Guid Id { get; set; }

    public DateTime UpdatedAt { get; set; }

    public IList<Position> Positions { get; set; } = [];

    public IList<Position> Comissions { get; set; } = [];

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions.DefaultOptions);

    public static Portfolio? FromJson(string json) => JsonSerializer.Deserialize<Portfolio>(json, JsonOptions.DefaultOptions);
}