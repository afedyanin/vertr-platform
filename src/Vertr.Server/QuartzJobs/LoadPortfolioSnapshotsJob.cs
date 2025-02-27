using MediatR;
using Quartz;
using Vertr.Application.Portfolios;

namespace Vertr.Server.QuartzJobs;

internal static class LoadPortfolioSnapshotsJobKeys
{
    public const string Name = "Load Tinvest portfolio snapshots job";
    public const string Group = "Tinvest";
    public const string Accounts = "accounts";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

public class LoadPortfolioSnapshotsJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<LoadPortfolioSnapshotsJob> _logger;

    public LoadPortfolioSnapshotsJob(
        IMediator mediator,
        ILogger<LoadPortfolioSnapshotsJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{LoadPortfolioSnapshotsJobKeys.Name} starting.");

        var dataMap = context.JobDetail.JobDataMap;
        var accountsString = dataMap.GetString(LoadPortfolioSnapshotsJobKeys.Accounts);

        var request = new LoadPortfolioSnapshotsRequest
        {
            Accounts = accountsString!.Split(','),
        };

        await _mediator.Send(request);
        _logger.LogInformation($"{LoadPortfolioSnapshotsJobKeys.Name} completed.");
    }
}
