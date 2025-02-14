using MediatR;
using Quartz;
using Vertr.Application.Candles;
using Vertr.Domain;

namespace Vertr.Server.QuartzJobs;

internal static class LoadTinvestCandlesJobKeys
{
    public const string Name = "load tinvest candles job";
    public const string Group = "tinvest";
    public const string Symbols = "symbols";
    public const string Interval = "interval";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

public class LoadTinvestCandlesJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<LoadTinvestCandlesJob> _logger;

    public LoadTinvestCandlesJob(
        IMediator mediator,
        ILogger<LoadTinvestCandlesJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Load Tinvest Candles Job starting.");

        var dataMap = context.JobDetail.JobDataMap;
        var symbolsString = dataMap.GetString(LoadTinvestCandlesJobKeys.Symbols);
        var intervalValue = dataMap.GetInt(LoadTinvestCandlesJobKeys.Interval);

        var request = new LoadTinvestCandlesRequest
        {
            Symbols = symbolsString!.Split(','),
            Interval = (CandleInterval)intervalValue
        };

        await _mediator.Send(request);
        _logger.LogInformation("Load Tinvest Candles Job completed.");
    }
}
