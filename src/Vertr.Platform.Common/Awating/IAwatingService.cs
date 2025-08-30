namespace Vertr.Platform.Common.Awating;
public interface IAwatingService<T> where T : struct
{
    public Task<T> WaitToComplete(T id, CancellationToken cancellationToken = default);

    public void SetCompleted(T id);
}
