namespace Vertr.Platform.BlazorUI.Components.Models;
public class StockModel
{
    public required string Symbol { get; set; }

    public decimal DayOpen { get; set; }

    public decimal DayLow { get; set; }

    public decimal DayHigh { get; set; }

    public string? LastChange { get; set; }

    public decimal Change { get; set; }

    public double PercentChange { get; set; }

    public DateTime UpdatedAt { get; set; }
}
