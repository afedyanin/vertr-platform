using MediatR;
using Quartz;
using Vertr.Application.Operations;

namespace Vertr.Server.QuartzJobs;

internal static class LoadOperationsJobKeys
{
    public const string Name = "Load Tinvest operations job";
    public const string Group = "Tinvest";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

public class LoadOperationsJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<LoadOperationsJob> _logger;

    public LoadOperationsJob(
        IMediator mediator,
        ILogger<LoadOperationsJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{LoadOperationsJobKeys.Name} starting.");

        await _mediator.Send(new LoadOperationsRequest());

        _logger.LogInformation($"{LoadOperationsJobKeys.Name} completed.");
    }
}
