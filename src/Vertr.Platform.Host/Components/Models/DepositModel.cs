namespace Vertr.Platform.Host.Components.Models;

public class DepositModel
{
    public DateTime? SelectedDate { get; set; } = DateTime.UtcNow;

    public DateTime? SelectedTime { get; set; }
    public decimal Amount { get; set; }

    public required string Currency { get; set; }

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
