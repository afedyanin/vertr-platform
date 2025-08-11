using MediatR;

namespace Vertr.Platform.Common.Jobs;

public interface IJobScheduler
{
    public string Enqueue(IRequest request, CancellationToken token = default);

    public string Schedule(IRequest request, TimeSpan delay, CancellationToken token = default);

    public string EnqueueAfter(string previousJobId, IRequest request, CancellationToken token = default);
}
