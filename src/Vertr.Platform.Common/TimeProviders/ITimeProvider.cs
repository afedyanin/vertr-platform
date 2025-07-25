namespace Vertr.Platform.Common.TimeProviders;

public interface ITimeProvider
{
    public DateTime GetCurrentTime();

    public void SetCurrentTime(DateTime time);
}
