using System.Collections.Concurrent;
using Vertr.Platform.Common.Awating;

namespace Vertr.Infrastructure.Common.Awaiting;
internal class AwatingService<T> : IAwatingService<T> where T : struct
{
    private readonly ConcurrentDictionary<T, TaskCompletionSource<T>> _commands = [];

    public Task<T> WaitToComplete(T id, CancellationToken cancellationToken = default)
    {
        // завершенную задачу нужно выдергивать из словаря?

        if (_commands.TryGetValue(id, out var existing))
        {
            return existing.Task;
        }

        var tcs = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        cancellationToken.Register(
            CancelCallback,
            new CancelCallbackContext(id, cancellationToken));

        var added = _commands.GetOrAdd(id, tcs);

        return added.Task;
    }

    public void SetCompleted(T id)
    {
        // Этот метод может вызваться раньше, чем WaitToComplete
        // Нужно обрабатывать такой кейс

        if (_commands.TryRemove(id, out var tcs))
        {
            _ = tcs.TrySetResult(id);
        }
    }

    private void CancelCallback(object? idObject)
    {
        var context = (CancelCallbackContext)idObject!;

        if (_commands.TryRemove(context.Id, out var tcs))
        {
            _ = tcs.TrySetCanceled(context.CancellationToken);
        }
    }

    private readonly record struct CancelCallbackContext(T Id, CancellationToken CancellationToken);
}
