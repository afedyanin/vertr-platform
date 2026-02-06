namespace Vertr.Clients.MoexApiClient.Models;

public record class IndexRate
{
    public required string Ticker { get; set; }

    public DateTime Time { get; set; }

    public decimal Value { get; set; }
}
