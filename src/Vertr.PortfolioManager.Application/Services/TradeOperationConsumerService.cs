using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Infrastructure.Common.Channels;
using Vertr.MarketData.Contracts.Interfaces.old;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application.Services;

internal class TradeOperationConsumerService : DataConsumerServiceBase<TradeOperation>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ITradeOperationRepository _tradeOperationRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<TradeOperationConsumerService> _logger;

    public TradeOperationConsumerService(
        IServiceProvider serviceProvider,
        ILogger<TradeOperationConsumerService> logger) : base(serviceProvider)
    {
        _portfolioRepository = serviceProvider.GetRequiredService<IPortfolioRepository>();
        _currencyRepository = serviceProvider.GetRequiredService<ICurrencyRepository>();
        _tradeOperationRepository = serviceProvider.GetRequiredService<ITradeOperationRepository>();
        _logger = logger;
    }

    protected override async Task Handle(TradeOperation data, CancellationToken cancellationToken = default)
    {
        await SaveOperation(data);
        await ApplyOperation(data);
    }

    private async Task ApplyOperation(TradeOperation operation)
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

#pragma warning disable IDE0010 // Add missing cases
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
#pragma warning restore IDE0010 // Add missing cases

        portfolio.UpdatedAt = operation.CreatedAt;
        _portfolioRepository.Save(portfolio);
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
        var currencyId = _currencyRepository.GetCurrencyId(currencyCode);

        if (currencyId == null)
        {
            _logger.LogDebug($"Detecting currency by instrumentId={instrumentId}");
            currencyId = await _currencyRepository.GetInstrumentCurrencyId(instrumentId);

        }

        return currencyId;
    }

    private async Task SaveOperation(TradeOperation operation)
    {
        if (operation == null || !operation.IsNew)
        {
            return;
        }

        var saved = await _tradeOperationRepository.Save(operation);

        if (saved)
        {
            return;
        }

        var message = $"Cannot save trade operation. OrderId={operation.OrderId}";
        throw new InvalidOperationException(message);
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
