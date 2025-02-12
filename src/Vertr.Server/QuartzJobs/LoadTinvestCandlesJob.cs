using MediatR;
using Quartz;
using Vertr.Application.Candles;
using Vertr.Domain;

namespace Vertr.Server.QuartzJobs;

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
        _logger.LogInformation("LoadTinvestCandlesJob starting.");
        var dataMap = context.JobDetail.JobDataMap;

        var symbolsString = dataMap.GetString("symbols");
        var intervalValue = dataMap.GetInt("interval");

        var request = new LoadTinvestCandlesRequest
        {
            Symbols = symbolsString!.Split(','),
            Interval = (CandleInterval)intervalValue
        };

        await _mediator.Send(request);
        _logger.LogInformation("LoadTinvestCandlesJob completed.");
    }
}
