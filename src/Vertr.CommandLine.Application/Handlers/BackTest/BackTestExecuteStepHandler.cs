using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Models.Requests.BackTest;
using Vertr.CommandLine.Models.Requests.Orders;
using Vertr.CommandLine.Models.Requests.Predictor;

namespace Vertr.CommandLine.Application.Handlers.BackTest;

public class BackTestExecuteStepHandler : IRequestHandler<BackTestExecuteStepRequest, BackTestExecuteStepResponse>
{
    private readonly IMediator _mediator;
    private readonly IMarketDataService _marketDataService;
    private readonly IPortfolioService _portfolioService;

    public BackTestExecuteStepHandler(
        IMarketDataService marketDataService,
        IPortfolioService portfolioService,
        IMediator mediator)
    {
        _marketDataService = marketDataService;
        _portfolioService = portfolioService;
        _mediator = mediator;
    }

    public async Task<BackTestExecuteStepResponse> Handle(BackTestExecuteStepRequest request, CancellationToken cancellationToken = default)
    {
        var rb = new BackTestExecuteStepResponseBuilder()
            .WithMarketTime(request.Time);

        var predictionRequest = new PredictionRequest
        {
            Time = request.Time,
            Symbol = request.Symbol,
            Predictor = request.Predictor,
            CandlesCount = request.PredictionRequestCandlesCount
        };

        var predictionResponse = await _mediator.Send(predictionRequest, cancellationToken);

        if (predictionResponse.HasErrors)
        {
            return rb
                .WithError(predictionResponse.Exception, $"Prediction failed with error: {predictionResponse.Message}")
                .Build();
        }

        if (!predictionResponse.PredictedPrice.HasValue)
        {
            return rb
                .WithMessage($"Prediction value is undefined. Message: {predictionResponse.Message}")
                .Build();
        }

        rb = rb
            .WithPredictedPrice(predictionResponse.PredictedPrice.Value)
            .WithCandle(predictionResponse.LastCandle);

        var marketPrice = await _marketDataService.GetMarketPrice(request.Symbol, request.Time, Models.PriceType.Mid);

        if (!marketPrice.HasValue)
        {
            return rb
                .WithMessage($"Market price is undefined.")
                .Build();
        }

        var direction = GetTradingDirection(marketPrice.Value, predictionResponse.PredictedPrice.Value, request.PriceThreshold);

        rb = rb
            .WithMarketPrice(marketPrice.Value)
            .WithSignal(direction);

        if (direction == 0)
        {
            return rb
                .WithMessage($"No trade direction selected.")
                .Build();
        }

        var tradingSignalRequest = new TradingSignalRequest
        {
            PortfolioId = request.PortfolioId,
            Symbol = request.Symbol,
            Direction = direction,
            MarketTime = request.Time,
            OpenPositionQty = request.OpenPositionQty,
            ComissionPercent = request.ComissionPercent,
        };

        var tradingSignalResponse = await _mediator.Send(tradingSignalRequest, cancellationToken);

        if (tradingSignalResponse.HasErrors)
        {
            return rb
                .WithError(tradingSignalResponse.Exception, $"Trading signal request failed with error: {tradingSignalResponse.Message}")
                .Build();
        }

        var trades = tradingSignalResponse.Trades;

        if (trades.Length <= 0)
        {
            return rb
                .WithMessage($"No trades received. Message={tradingSignalResponse.Message}")
                .Build();
        }

        rb = rb.WithTrades(trades);

        var positions = _portfolioService.Update(
            request.PortfolioId,
            request.Symbol,
            trades,
            request.CurrencyCode);

        if (positions.Length <= 0)
        {
            return rb
                .WithMessage($"No positions updated.")
                .Build();
        }

        return rb
            .WithPositions(positions)
            .Build();
    }

    private static Direction GetTradingDirection(decimal marketPrice, decimal predictedNextPrice, decimal treshold)
    {
        if (marketPrice == decimal.Zero || predictedNextPrice == decimal.Zero)
        {
            return Direction.Hold;
        }

        var delta = (predictedNextPrice - marketPrice) / marketPrice;

        if (Math.Abs(delta) <= treshold)
        {
            return Direction.Hold;
        }

        return delta > 0 ? Direction.Buy : Direction.Sell;
    }
}