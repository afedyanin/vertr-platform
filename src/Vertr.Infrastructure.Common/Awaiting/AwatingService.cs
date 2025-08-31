using System.Collections.Concurrent;
using Vertr.Platform.Common.Awating;

namespace Vertr.Infrastructure.Common.Awaiting;
public class AwatingService<T> : IAwatingService<T> where T : struct
{
    private readonly ConcurrentDictionary<T, TaskCompletionSource<T>> _commands = [];

    private readonly HashSet<T> _keysAdded = [];
    private readonly HashSet<T> _keysCompleted = [];
    private readonly HashSet<T> _keysCancelled = [];

    public Task<T> WaitToComplete(T id, CancellationToken cancellationToken = default)
    {
        if (_commands.TryGetValue(id, out var existing))
        {
            if (existing.Task.IsCompleted)
            {
                if (_commands.TryRemove(id, out var _))
                {
                    _keysCompleted.Add(id);
                }
            }

            return existing.Task;
        }

        var tcs = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        cancellationToken.Register(
            CancelCallback,
            new CancelCallbackContext(id, cancellationToken));

        var added = _commands.GetOrAdd(id, tcs);
        _keysAdded.Add(id);

        return added.Task;
    }

    public void SetCompleted(T id)
    {
        if (_commands.TryRemove(id, out var tcs))
        {
            _ = tcs.TrySetResult(id);
            _keysCompleted.Add(id);
            return;
        }

        // Этот метод может вызваться раньше, чем WaitToComplete
        // Поэтому добавим завершенную задачу

        var tcsCompleted = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        if (_commands.TryAdd(id, tcsCompleted))
        {
            tcsCompleted.SetResult(id);
            _keysAdded.Add(id);
        }
    }

    private void CancelCallback(object? idObject)
    {
        var context = (CancelCallbackContext)idObject!;

        if (_commands.TryRemove(context.Id, out var tcs))
        {
            _ = tcs.TrySetCanceled(context.CancellationToken);
            _keysCancelled.Add(context.Id);
        }
    }

    public AwatingServiceStats<T> GetStatistics()
        => new AwatingServiceStats<T>
        {
            KeysAdded = [.. _keysAdded],
            KeysCancelled = [.. _keysCancelled],
            KeysCompleted = [.. _keysCompleted],
            KeysAwaiting = [.. _commands.Keys]
        };

    private readonly record struct CancelCallbackContext(T Id, CancellationToken CancellationToken);
}
