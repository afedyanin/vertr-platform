using MediatR;
using Microsoft.Extensions.Options;
using Quartz;
using Vertr.Application.Signals;
using Vertr.Domain.Settings;

namespace Vertr.Server.QuartzJobs;

internal static class ExecuteOrdersJobKeys
{
    public const string Name = "Execute orders by trading signals";
    public const string Group = "Tinvest";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

public class ExecuteOrdersJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<ExecuteOrdersJob> _logger;
    private readonly AccountStrategySettings _accountStrategySettings;

    public ExecuteOrdersJob(
        IMediator mediator,
        IOptions<AccountStrategySettings> accountStrategyOptions,
        ILogger<ExecuteOrdersJob> logger)
    {
        _mediator = mediator;
        _accountStrategySettings = accountStrategyOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{ExecuteOrdersJobKeys.Name} starting.");

        var request = new ProcessSignalsRequest
        {
            Accounts = [.. _accountStrategySettings.SignalMappings.Keys],
        };

        await _mediator.Send(request);

        _logger.LogInformation($"{ExecuteOrdersJobKeys.Name} completed.");
    }
}
