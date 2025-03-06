using MediatR;
using Microsoft.Extensions.Options;
using Quartz;
using Vertr.Application.Portfolios;
using Vertr.Domain.Settings;

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
    private readonly AccountStrategySettings _accountStrategySettings;

    public LoadPortfolioSnapshotsJob(
        IMediator mediator,
        IOptions<AccountStrategySettings> accountStrategyOptions,
        ILogger<LoadPortfolioSnapshotsJob> logger)
    {
        _mediator = mediator;
        _accountStrategySettings = accountStrategyOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{LoadPortfolioSnapshotsJobKeys.Name} starting.");

        var request = new LoadPortfolioSnapshotsRequest
        {
            Accounts = [.. _accountStrategySettings.SignalMappings.Keys],
        };

        await _mediator.Send(request);
        _logger.LogInformation($"{LoadPortfolioSnapshotsJobKeys.Name} completed.");
    }
}
