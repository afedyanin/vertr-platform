using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application.Services;
public class TradeOperationService : ITradeOperationService
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IStaticMarketDataProvider _staticMarketDataProvider;
    private readonly ILogger<TradeOperationService> _logger;

    public TradeOperationService(
        IPortfolioRepository portfolioRepository,
        IStaticMarketDataProvider staticMarketDataProvider,
        ILogger<TradeOperationService>? logger = null)
    {
        _portfolioRepository = portfolioRepository;
        _staticMarketDataProvider = staticMarketDataProvider;
        _logger = logger ?? NullLoggerFactory.Instance.CreateLogger<TradeOperationService>();
    }

    public async Task<Portfolio> ApplyOperation(TradeOperation operation)
    {
        var portfolioIdentity = new PortfolioIdentity(operation.AccountId, operation.SubAccountId);
        var portfolio = _portfolioRepository.GetPortfolio(portfolioIdentity);

        portfolio ??= new Portfolio
        {
            Identity = portfolioIdentity,
        };

        var currencyPosition = await GetCurrencyPosition(portfolio, operation);
        var operationPosition = GetOrCreatePosition(portfolio, operation.InstrumentId);
        var operationAmount = Math.Abs(operation.Amount.Value);

        switch (operation.OperationType)
        {
            case TradeOperationType.Buy:
                operationPosition.Balance += operation.Quantity ?? 0L;
                currencyPosition.Balance -= operationAmount;
                break;
            case TradeOperationType.Sell:
                operationPosition.Balance -= operation.Quantity ?? 0L;
                currencyPosition.Balance += operationAmount;
                break;
            case TradeOperationType.BrokerFee:
                currencyPosition.Balance -= operationAmount;
                break;
            case TradeOperationType.Input:
                currencyPosition.Balance += operationAmount;
                break;
            case TradeOperationType.Output:
                currencyPosition.Balance -= operationAmount;
                break;
            case TradeOperationType.PositionOverride:
                operationPosition.Balance = operationAmount;
                break;
            default:
                throw new InvalidOperationException($"Unsupported TradeOperationType={operation.OperationType}");
        }

        portfolio.UpdatedAt = operation.CreatedAt;

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
        var currencyId = _staticMarketDataProvider.GetCurrencyId(currencyCode);

        if (currencyId == null)
        {
            _logger.LogDebug($"Detecting currency by instrumentId={instrumentId}");
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

            portfolio.Positions.Add(position);
        }

        return position;
    }
}
