using MediatR;
using Microsoft.Extensions.Options;
using Quartz;
using Vertr.Application.Operations;
using Vertr.Domain.Settings;

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
    private readonly AccountStrategySettings _accountStrategySettings;

    public LoadOperationsJob(
        IMediator mediator,
        IOptions<AccountStrategySettings> accountStrategyOptions,
        ILogger<LoadOperationsJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _accountStrategySettings = accountStrategyOptions.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{LoadOperationsJobKeys.Name} starting.");

        var request = new LoadOperationsRequest
        {
            Accounts = [.. _accountStrategySettings.SignalMappings.Keys],
        };

        await _mediator.Send(request);
        _logger.LogInformation($"{LoadOperationsJobKeys.Name} completed.");
    }
}
