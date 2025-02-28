using MediatR;
using Microsoft.Extensions.Options;
using Quartz;
using Vertr.Adapters.Tinvest;
using Vertr.Application.Portfolios;

namespace Vertr.Server.QuartzJobs;

internal static class LoadPortfolioSnapshotsJobKeys
{
    public const string Name = "Load Tinvest portfolio snapshots job";
    public const string Group = "Tinvest";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

public class LoadPortfolioSnapshotsJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<LoadPortfolioSnapshotsJob> _logger;
    private readonly TinvestSettings _tinvestSettings;

    public LoadPortfolioSnapshotsJob(
        IMediator mediator,
        IOptions<TinvestSettings> tinvestOptions,
        ILogger<LoadPortfolioSnapshotsJob> logger)
    {
        _mediator = mediator;
        _tinvestSettings = tinvestOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{LoadPortfolioSnapshotsJobKeys.Name} starting.");

        var request = new LoadPortfolioSnapshotsRequest
        {
            Accounts = _tinvestSettings.Accounts,
        };

        await _mediator.Send(request);
        _logger.LogInformation($"{LoadPortfolioSnapshotsJobKeys.Name} completed.");
    }
}
