using MediatR;
using Quartz;
using Vertr.Application.Candles;
using Vertr.Domain.Enums;
using Vertr.Domain.Settings;

namespace Vertr.Server.QuartzJobs;

internal static class UpdateTinvestCandlesJobKeys
{
    public const string Name = "Update latest candles job";
    public const string Group = "Tinvest";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

public class UpdateLastCandlesJob : IJob
{
    private readonly IMediator _mediator;
    private readonly AccountStrategySettings _accountStrategySettings;
    private readonly ILogger<UpdateLastCandlesJob> _logger;

    public UpdateLastCandlesJob(
        IMediator mediator,
        AccountStrategySettings accountStrategySettings,
        ILogger<UpdateLastCandlesJob> logger)
    {
        _mediator = mediator;
        _accountStrategySettings = accountStrategySettings;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{UpdateTinvestCandlesJobKeys.Name} starting.");

        var symbols = ComposeSymbols();

        var request = new UpdateLastCandlesRequest
        {
            Symbols = symbols,
        };

        await _mediator.Send(request);
        _logger.LogInformation($"{UpdateTinvestCandlesJobKeys.Name} completed.");
    }

    private IEnumerable<(string, CandleInterval)> ComposeSymbols()
    {
        var symbols = new HashSet<(string, CandleInterval)>();

        foreach (var strategySettingsList in _accountStrategySettings.SignalMappings.Values)
        {
            if (strategySettingsList == null)
            {
                continue;
            }

            foreach (var item in strategySettingsList)
            {
                symbols.Add((item.Symbol, item.Interval));
            }
        }

        return symbols;
    }
}
