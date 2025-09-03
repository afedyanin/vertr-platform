using Vertr.MarketData.Contracts;

namespace Vertr.Platform.BlazorUI.Components.Models;

public class CloseReversePostionModel
{
    public required Instrument Instrument { get; set; }

    public decimal Balance { get; set; }

    public DateTime? SelectedDate { get; set; } = DateTime.UtcNow;

    public DateTime? SelectedTime { get; set; }

    public bool OrderExecutionSimulated { get; set; }

    public decimal Price { get; set; }

    public DateTime ComposeDate()
    {
        var fromDay = SelectedDate.HasValue ? SelectedDate.Value.Date : DateTime.UtcNow.Date;

        if (SelectedTime.HasValue)
        {
            return new DateTime(fromDay.Year, fromDay.Month, fromDay.Day, SelectedTime.Value.Hour, SelectedTime.Value.Minute, 0, DateTimeKind.Utc);
        }

        return new DateTime(fromDay.Year, fromDay.Month, fromDay.Day, 0, 0, 0, DateTimeKind.Utc);
    }

}
