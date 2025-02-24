using MediatR;
using Quartz;
using Vertr.Application.Signals;
using Vertr.Domain;
using Vertr.Domain.Enums;

namespace Vertr.Server.QuartzJobs;

internal static class GenerateSignalsJobKeys
{
    public const string Name = "Generate trading signals job";
    public const string Group = "vertr";
    public const string Symbols = "symbols";
    public const string Interval = "interval";
    public const string PredictorType = "predictor_type";
    public const string Sb3Algo = "sb3_algo";

    public static readonly JobKey Key = new JobKey(Name, Group);
}

public class GenerateSignalsJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<GenerateSignalsJob> _logger;

    public GenerateSignalsJob(
        IMediator mediator,
        ILogger<GenerateSignalsJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{GenerateSignalsJobKeys.Name} starting.");

        var dataMap = context.JobDetail.JobDataMap;
        var symbolsString = dataMap.GetString(GenerateSignalsJobKeys.Symbols);
        var intervalValue = dataMap.GetInt(GenerateSignalsJobKeys.Interval);
        var predictorType = new PredictorType(dataMap.GetString(GenerateSignalsJobKeys.PredictorType)!);
        var sb3Algo = new Sb3Algo(dataMap.GetString(GenerateSignalsJobKeys.Sb3Algo)!);

        var request = new GenerateSignalsRequest
        {
            Symbols = symbolsString!.Split(','),
            Interval = (CandleInterval)intervalValue,
            PredictorType = predictorType,
            Sb3Algo = sb3Algo
        };

        await _mediator.Send(request);
        _logger.LogInformation($"{GenerateSignalsJobKeys.Name} completed.");
    }
}
