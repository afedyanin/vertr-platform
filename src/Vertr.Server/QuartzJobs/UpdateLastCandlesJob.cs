using MediatR;
using Quartz;
using Vertr.Application.Candles;
using Vertr.Domain.Enums;

namespace Vertr.Server.QuartzJobs;

internal static class UpdateTinvestCandlesJobKeys
{
    public const string Name = "Update latest candles job";
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
        _logger.LogInformation($"{UpdateTinvestCandlesJobKeys.Name} starting.");

        var dataMap = context.JobDetail.JobDataMap;
        var symbolsString = dataMap.GetString(UpdateTinvestCandlesJobKeys.Symbols);
        var intervalValue = dataMap.GetInt(UpdateTinvestCandlesJobKeys.Interval);

        var request = new UpdateLastCandlesRequest
        {
            Symbols = symbolsString!.Split(','),
            Interval = (CandleInterval)intervalValue
        };

        await _mediator.Send(request);
        _logger.LogInformation($"{UpdateTinvestCandlesJobKeys.Name} completed.");
    }
}
