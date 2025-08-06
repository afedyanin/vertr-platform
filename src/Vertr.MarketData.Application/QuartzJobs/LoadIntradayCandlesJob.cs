using Microsoft.Extensions.Logging;
using Quartz;

namespace Vertr.MarketData.Application.QuartzJobs;

internal static class LoadIntradayCandlesJobKeys
{
    public const string Name = "Load intraday candles job";
    public const string Group = "Market Data";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

internal class LoadIntradayCandlesJob : IJob
{
    private readonly ILogger<LoadIntradayCandlesJob> _logger;

    public LoadIntradayCandlesJob(
        ILogger<LoadIntradayCandlesJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{LoadIntradayCandlesJobKeys.Name} starting.");

        await Task.Delay(3000);

        _logger.LogInformation($"{LoadIntradayCandlesJobKeys.Name} completed.");
    }
}
