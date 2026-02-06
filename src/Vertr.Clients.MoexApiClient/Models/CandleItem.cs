using CsvHelper.Configuration.Attributes;

namespace Vertr.Clients.MoexApiClient.Models;

internal record class CandleItem
{
    [Name("open")]
    public decimal Open { get; set; }
    [Name("close")]
    public decimal Close { get; set; }
    [Name("high")]
    public decimal High { get; set; }
    [Name("low")]
    public decimal Low { get; set; }
    [Name("value")]
    public decimal Value { get; set; }
    [Name("volume")]
    public decimal Volume { get; set; }
    [Name("begin")]
    public DateTime Begin { get; set; }
    [Name("end")]
    public DateTime End { get; set; }
}
