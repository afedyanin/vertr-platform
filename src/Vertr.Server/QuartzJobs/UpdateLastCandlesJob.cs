using MediatR;
using Quartz;
using Vertr.Application.Candles;
using Vertr.Domain;

namespace Vertr.Server.QuartzJobs;

internal static class LoadTinvestCandlesJobKeys
{
    public const string Name = "Load latest candles job";
    public const string Group = "Tinvest";
    public const string Symbols = "symbols";
    public const string Interval = "interval";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

public class UpdateLastCandlesJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateLastCandlesJob> _logger;

    public UpdateLastCandlesJob(
        IMediator mediator,
        ILogger<UpdateLastCandlesJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{LoadTinvestCandlesJobKeys.Name} starting.");

        var dataMap = context.JobDetail.JobDataMap;
        var symbolsString = dataMap.GetString(LoadTinvestCandlesJobKeys.Symbols);
        var intervalValue = dataMap.GetInt(LoadTinvestCandlesJobKeys.Interval);

        var request = new UpdateLastCandlesRequest
        {
            Symbols = symbolsString!.Split(','),
            Interval = (CandleInterval)intervalValue
        };

        await _mediator.Send(request);
        _logger.LogInformation($"{LoadTinvestCandlesJobKeys.Name} completed.");
    }
}
