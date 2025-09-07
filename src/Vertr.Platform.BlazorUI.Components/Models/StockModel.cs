namespace Vertr.Platform.BlazorUI.Components.Models;
public class StockModel
{
    private decimal _lastChange;

    public required string Symbol { get; set; }

    public decimal DayOpen { get; set; }

    public decimal DayLow { get; set; }

    public decimal DayHigh { get; set; }

    public decimal LastChange
    {
        get
        {
            return _lastChange;
        }
        set
        {
            _lastChange = value;
            Updated?.Invoke(this);
        }
    }

    public decimal Change { get; set; }

    public double PercentChange { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Func<StockModel, Task>? Updated;

}
