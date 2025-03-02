using MediatR;
using Microsoft.Extensions.Options;
using Quartz;
using Vertr.Application.Signals;
using Vertr.Domain.Settings;

namespace Vertr.Server.QuartzJobs;

internal static class GenerateSignalsJobKeys
{
    public const string Name = "Generate trading signals job";
    public const string Group = "vertr";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

public class GenerateSignalsJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<GenerateSignalsJob> _logger;
    private readonly AccountStrategySettings _accountStrategySettings;

    public GenerateSignalsJob(
        IMediator mediator,
        IOptions<AccountStrategySettings> accountStrategyOptions,
        ILogger<GenerateSignalsJob> logger)
    {
        _mediator = mediator;
        _accountStrategySettings = accountStrategyOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{GenerateSignalsJobKeys.Name} starting.");

        var strategies = ComposeStrategySettings();

        var request = new GenerateSignalsRequest
        {
            Strategies = strategies,
        };

        await _mediator.Send(request);
        _logger.LogInformation($"{GenerateSignalsJobKeys.Name} completed.");
    }

    private IEnumerable<StrategySettings> ComposeStrategySettings()
    {
        var result = new HashSet<StrategySettings>();

        foreach (var strategySettingsList in _accountStrategySettings.SignalMappings.Values)
        {
            if (strategySettingsList == null)
            {
                continue;
            }

            foreach (var item in strategySettingsList)
            {
                result.Add(item);
            }
        }

        return result;
    }
}
