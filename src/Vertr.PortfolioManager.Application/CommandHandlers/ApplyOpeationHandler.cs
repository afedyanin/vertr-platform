using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application.CommandHandlers;

internal class ApplyOperationRequest : IRequest
{
    public required TradeOperation Operation { get; init; }
}

internal class ApplyOpeationHandler : IRequestHandler<ApplyOperationRequest>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<ApplyOpeationHandler> _logger;

    public ApplyOpeationHandler(
        IPortfolioRepository portfolioRepository,
        ICurrencyRepository currencyRepository,
        ILogger<ApplyOpeationHandler> logger)
    {
        _portfolioRepository = portfolioRepository;
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task Handle(ApplyOperationRequest request, CancellationToken cancellationToken = default)
    {
        var operation = request.Operation;

        var portfolio = await _portfolioRepository.GetById(operation.PortfolioId);

        portfolio ??= new Portfolio
        {
            Id = operation.PortfolioId,
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
        await _portfolioRepository.Save(portfolio);
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
        var currencyId = await _currencyRepository.GetCurrencyId(currencyCode);

        if (currencyId == null)
        {
            _logger.LogDebug($"Detecting currency by instrumentId={instrumentId}");
            currencyId = await _currencyRepository.GetInstrumentCurrencyId(instrumentId);

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
