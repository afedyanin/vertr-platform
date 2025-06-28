using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;

internal class TradeOperationsHandler : IRequestHandler<TradeOperationsRequest>
{
    private readonly ITradeOperationRepository _operationEventRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IStaticMarketDataProvider _staticMarketDataProvider;
    private readonly ILogger<TradeOperationsHandler> _logger;

    public TradeOperationsHandler(
        ITradeOperationRepository operationEventRepository,
        IPortfolioRepository portfolioRepository,
        IStaticMarketDataProvider staticMarketDataProvider,
        ILogger<TradeOperationsHandler> logger)
    {
        _operationEventRepository = operationEventRepository;
        _portfolioRepository = portfolioRepository;
        _staticMarketDataProvider = staticMarketDataProvider;
        _logger = logger;
    }

    public async Task Handle(TradeOperationsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Order operations received.");

        var saved = await _operationEventRepository.Save(request.Operations);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save operation events.");
        }

        foreach (var operation in request.Operations)
        {
            var portfolioIdentity = new PortfolioIdentity(operation.AccountId, operation.SubAccountId);
            var portfolio = _portfolioRepository.GetPortfolio(portfolioIdentity);

            portfolio ??= new Portfolio
            {
                Identity = portfolioIdentity,
                UpdatedAt = DateTime.UtcNow,
            };

            var currencyPosition = await GetCurrencyPosition(portfolio, operation);
            var operationPosition = GetOrCreatePosition(portfolio, operation.InstrumentId);

            var updated = operation.OperationType switch
            {
                TradeOperationType.Buy => ApplyBuy(portfolio, operation),
                TradeOperationType.Sell => ApplySell(portfolio, operation),
                TradeOperationType.Input => ApplyInput(portfolio, operation),
                TradeOperationType.Output => ApplyOutput(portfolio, operation),
                TradeOperationType.ServiceFee => ApplyFee(portfolio, operation),
                TradeOperationType.BrokerFee => ApplyFee(portfolio, operation),
                _ => throw new NotImplementedException($"OperationType={operation.OperationType} is not implemented.")
            };

            _portfolioRepository.Save(updated);
        }
    }

    private Portfolio ApplyBuy(Position operationPosition, Position currencyPosition)
    {
        operationPosition.Balance += operation.Quantity ?? 0L;

        return portfolio;
    }

    private Portfolio ApplySell(Portfolio portfolio, TradeOperation operation)
    {
        var operationPosition = GetOrCreatePosition(portfolio, operation.InstrumentId);
        operationPosition.Balance -= operation.Quantity ?? 0L;

        return portfolio;
    }

    private Portfolio ApplyFee(Portfolio portfolio, TradeOperation operation)
    {
        var currencyId = a
        //Debug.Assert(position.InstrumentId == rubPosition.InstrumentId, $"Assert Failed: Instrumentd={position.InstrumentId} RubInstrumentId={rubPosition.InstrumentId}");
        return portfolio;
    }

    private Portfolio ApplyInput(Portfolio portfolio, TradeOperation operation)
    {
        //Debug.Assert(position.InstrumentId == rubPosition.InstrumentId, $"Assert Failed: Instrumentd={position.InstrumentId} RubInstrumentId={rubPosition.InstrumentId}");
        return portfolio;
    }

    private Portfolio ApplyOutput(Portfolio portfolio, TradeOperation operation)
    {
        //Debug.Assert(position.InstrumentId == rubPosition.InstrumentId, $"Assert Failed: Instrumentd={position.InstrumentId} RubInstrumentId={rubPosition.InstrumentId}");
        return portfolio;
    }

    private async Task<Position> GetCurrencyPosition(Portfolio portfolio, TradeOperation operation)
    {
        var currencyId = await GetCurrrencyId(operation.Amount.Currency, operation.InstrumentId);

        if (currencyId == null)
        {
            throw new InvalidOperationException("Currency is not defined.");
        }

        return GetOrCreatePosition(portfolio, currencyId.Value);
    }

    private async Task<Guid?> GetCurrrencyId(string currencyCode, Guid instrumentId)
    {
        var currencyId = await _staticMarketDataProvider.GetCurrencyId(currencyCode);

        if (currencyId == null)
        {
            currencyId = await _staticMarketDataProvider.GetInstrumentCurrencyId(instrumentId);
        }

        return currencyId;
    }

    private static Position GetOrCreatePosition(Portfolio portfolio, Guid instrumentId)
    {
        var position = portfolio.Positions.FirstOrDefault(p => p.InstrumentId == instrumentId);

        if (position == null)
        {
            position = new Position
            {
                InstrumentId = instrumentId,
                Balance = decimal.Zero,
            };
        }

        return position;
    }
}
