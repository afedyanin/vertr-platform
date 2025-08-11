using Hangfire;
using MediatR;
using Vertr.Platform.Common.Jobs;

namespace Vertr.Infrastructure.Hangfire;

internal class HangfireJobScheduler : IJobScheduler
{
    public string Enqueue(IRequest request, CancellationToken token = default)
        => BackgroundJob.Enqueue<IMediator>(m => m.Send(request, token));

    public string EnqueueAfter(string previousJobId, IRequest request, CancellationToken token = default)
        => BackgroundJob.ContinueJobWith<IMediator>(previousJobId, m => m.Send(request, token));

    public string Schedule(IRequest request, TimeSpan delay, CancellationToken token = default)
        => BackgroundJob.Schedule<IMediator>(m => m.Send(request, token), delay);
}
