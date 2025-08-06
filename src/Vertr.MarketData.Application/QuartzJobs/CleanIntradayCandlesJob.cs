using Microsoft.Extensions.Logging;
using Quartz;

namespace Vertr.MarketData.Application.QuartzJobs;

internal static class CleanIntradayCandlesJobKeys
{
    public const string Name = "Clean intraday candles job";
    public const string Group = "Market Data";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

internal class CleanIntradayCandlesJob : IJob
{
    private readonly ILogger<CleanIntradayCandlesJob> _logger;

    public CleanIntradayCandlesJob(
        ILogger<CleanIntradayCandlesJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{CleanIntradayCandlesJobKeys.Name} starting.");

        await Task.Delay(3000);

        _logger.LogInformation($"{CleanIntradayCandlesJobKeys.Name} completed.");
    }
}
