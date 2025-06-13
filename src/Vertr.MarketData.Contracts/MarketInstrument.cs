namespace Vertr.MarketData.Contracts;

public class MarketInstrument
{
    public Guid Id { get; set; }

    public required string Symbol { get; set; }

    public required string ClassCode { get; set; }

    public required string Name { get; set; }

    public string? Isin { get; set; }

    public string? Currency { get; set; }

    public decimal? LotSize { get; set; }

    public string? ExternalId { get; set; }
}
