namespace Vertr.Platform.Common.TimeProviders;
internal class RealTimeProvider : ITimeProvider
{
    public DateTime GetCurrentTime()
        => DateTime.UtcNow;

    public void SetCurrentTime(DateTime time)
    {
        // Just ignore setting
    }
}
