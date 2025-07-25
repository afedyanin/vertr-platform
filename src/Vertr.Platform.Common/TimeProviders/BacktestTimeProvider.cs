namespace Vertr.Platform.Common.TimeProviders;

internal class BacktestTimeProvider : ITimeProvider
{
    private DateTime _dateTime = DateTime.UtcNow;

    public DateTime GetCurrentTime()
        => _dateTime;

    public void SetCurrentTime(DateTime time)
    {
        _dateTime = time;
    }
}
