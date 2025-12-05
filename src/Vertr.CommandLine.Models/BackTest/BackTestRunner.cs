using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Helpers;
using Vertr.CommandLine.Models.Requests.BackTest;
using Vertr.Common.Mediator;

namespace Vertr.CommandLine.Models.BackTest;

public class BackTestRunner
{
    private readonly IMediator _mediator;
    private readonly IMarketDataService _marketDataService;
    private readonly IPortfolioService _portfolioService;
    private readonly ILogger _logger;
    private readonly Dictionary<string, CandleRange?> _candleRanges = [];

    public BackTestRunner(
        IMarketDataService marketDataService,
        IPortfolioService portfolioService,
        IMediator mediator,
        ILogger logger)
    {
        _marketDataService = marketDataService;
        _portfolioService = portfolioService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task InitMarketData(FileDataSource[] dataSources)
    {
        _candleRanges.Clear();

        foreach (var dataSource in dataSources)
        {
            var candles = CsvImporter.LoadCandles(dataSource.FilePath);
            await _marketDataService.LoadData(dataSource.Symbol, [.. candles]);
            _candleRanges[dataSource.Symbol] = await _marketDataService.GetCandleRange(dataSource.Symbol);
        }
    }

    public async Task<BackTestResult> Run(BackTestParams backTestParams)
    {
        _candleRanges.TryGetValue(backTestParams.Symbol, out var candleRange);
        Trace.Assert(candleRange != null);

        var result = new BackTestResult();

        var stepCount = 0;
        var closeTime = candleRange.LastDate;
        var maxSteps = backTestParams.Steps > 0 ? Math.Min(backTestParams.Steps + backTestParams.Skip, candleRange.Count - 1) :
            candleRange.Count - 1; // оставить одну свечку, чтобы закрыть позицию

        var timeIndex = await _marketDataService.GetEnumerable(backTestParams.Symbol);
        foreach (var timeStep in timeIndex)
        {
            var date = DateOnly.FromDateTime(timeStep.Date);

            if (backTestParams.Intraday.HasValue && date != backTestParams.Intraday.Value)
            {
                continue;
            }

            if (stepCount++ >= maxSteps)
            {
                closeTime = timeStep;
                break;
            }

            if (stepCount < backTestParams.Skip)
            {
                continue;
            }

            result.Items[timeStep] = await ExecuteStep(timeStep, backTestParams);
        }

        result.FinalClosePositionsResult = await ClosePositionsStep(closeTime, backTestParams);
        result.Positions = _portfolioService.GetPositions(backTestParams.PortfolioId);

        return result;
    }

    public async Task<IEnumerable<BackTestResult>> RunBatch(BackTestParams backTestParams, int batchCount)
    {
        _candleRanges.TryGetValue(backTestParams.Symbol, out var candleRange);
        Trace.Assert(candleRange != null);

        var results = new List<BackTestResult>();
        foreach (var btParams in CreateBackTestBatchParams(batchCount, backTestParams, candleRange, backTestParams.Intraday))
        {
            var res = await Run(btParams);
            results.Add(res);
        }

        return results;
    }

    public async Task<Dictionary<DateOnly, IEnumerable<BackTestResult>>> RunBatchByDay(BackTestParams backTestParams, int batchCount)
    {
        var ranges = await _marketDataService.GetCandleRanges(backTestParams.Symbol);

        var dailyRes = new Dictionary<DateOnly, IEnumerable<BackTestResult>>();

        foreach (var range in ranges)
        {
            var results = new List<BackTestResult>();

            foreach (var btParams in CreateBackTestBatchParams(batchCount, backTestParams, range.Value, range.Key))
            {
                var res = await Run(btParams);
                results.Add(res);
            }

            dailyRes[range.Key] = results;
        }

        return dailyRes;
    }

    private IEnumerable<BackTestParams> CreateBackTestBatchParams(
        int paramsCount,
        BackTestParams template,
        CandleRange candleRange,
        DateOnly? intraday)
    {
        var res = new List<BackTestParams>();

        for (var i = 0; i <= paramsCount; i++)
        {
            var btParams = new BackTestParams
            {
                PortfolioId = Guid.NewGuid(),
                Symbol = template.Symbol,
                CurrencyCode = template.CurrencyCode,
                Steps = template.Steps,
                Skip = Random.Shared.Next(0, candleRange.Count - template.Steps),
                OpenPositionQty = template.OpenPositionQty,
                ComissionPercent = template.ComissionPercent,
                PriceThreshold = template.PriceThreshold,
                Predictor = template.Predictor,
                Intraday = intraday,
                PredictionRequestCandlesCount = template.PredictionRequestCandlesCount,
            };

            res.Add(btParams);
        }

        return res;
    }

    private async Task<Dictionary<string, object>> ExecuteStep(DateTime timeStep, BackTestParams backTestParams)
    {
        var request = new BackTestExecuteStepRequest
        {
            Time = timeStep,
            Symbol = backTestParams.Symbol,
            PortfolioId = backTestParams.PortfolioId,
            CurrencyCode = backTestParams.CurrencyCode,
            OpenPositionQty = backTestParams.OpenPositionQty,
            ComissionPercent = backTestParams.ComissionPercent,
            PriceThreshold = backTestParams.PriceThreshold,
            Predictor = backTestParams.Predictor,
            PredictionRequestCandlesCount = backTestParams.PredictionRequestCandlesCount,
        };

        var response = await _mediator.Send(request);

        if (response.HasErrors)
        {
            _logger.LogError(response.Exception, $"Step {timeStep:O}. Error:{response.Message}");
        }

        return response.Items;
    }

    private async Task<Dictionary<string, object>> ClosePositionsStep(DateTime closeDate, BackTestParams backTestParams)
    {
        var closeRequest = new BackTestClosePositionRequest
        {
            MarketTime = closeDate,
            Symbol = backTestParams.Symbol,
            PortfolioId = backTestParams.PortfolioId,
            CurrencyCode = backTestParams.CurrencyCode,
            ComissionPercent = backTestParams.ComissionPercent
        };

        var closeResponse = await _mediator.Send(closeRequest);

        if (closeResponse.HasErrors)
        {
            _logger.LogError(closeResponse.Exception, $"Close positions step. Error:{closeResponse.Message}");
        }

        return closeResponse.Items;
    }
}