using System.Text;

namespace Vertr.CommandLine.Models;

public record class Position
{
    public Guid PortfolioId { get; init; }

    public required string Symbol { get; init; }

    public decimal Qty { get; set; }

    public Position ClonePosition()
        => new()
        {
            PortfolioId = PortfolioId,
            Symbol = Symbol,
            Qty = Qty,
        };

    public static string GetTradingPositionKey(string currencyCode) => $"{currencyCode}.trading";
    public static string GetComissionsPositionKey(string currencyCode) => $"{currencyCode}.comissions";
}

public record class PositionSummary
{
    public decimal TradingAmount { get; init; }

    public decimal Comissions { get; init; }

    public decimal TotalAmount => TradingAmount + Comissions;

    public Dictionary<string, decimal> Holdings { get; init; } = [];

    public override string? ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Trading={TradingAmount:c}");
        sb.AppendLine($"Comissions={Comissions:c}");
        sb.AppendLine($"Total={TotalAmount:c}");

        if (Holdings.Count > 0)
        {
            var holdings = string.Join(",", Holdings.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            sb.AppendLine($"Holdings={holdings}");
        }

        return sb.ToString();
    }
}