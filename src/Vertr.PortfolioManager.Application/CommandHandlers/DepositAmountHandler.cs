using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Channels;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Commands;

namespace Vertr.PortfolioManager.Application.CommandHandlers;

internal class DepositAmountHandler : IRequestHandler<DepositAmountRequest>
{
    private readonly IDataProducer<TradeOperation> _tradeOperationsProducer;

    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<DepositAmountHandler> _logger;

    public DepositAmountHandler(
        IDataProducer<TradeOperation> tradeOperationsProducer,
        ICurrencyRepository currencyRepository,
        ILogger<DepositAmountHandler> logger)
    {
        _tradeOperationsProducer = tradeOperationsProducer;
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    public async Task Handle(DepositAmountRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Pay in operation received.");

        var instrumentId = await _currencyRepository.GetCurrencyId(request.Amount.Currency);

        if (instrumentId == null)
        {
            throw new InvalidOperationException($"Unknown currecncy={request.Amount.Currency}");
        }

        var opPayIn = new TradeOperation
        {
            CreatedAt = request.CreatedAt,
            OperationType = TradeOperationType.Input,
            AccountId = request.AccountId,
            PortfolioId = request.PortfolioId,
            OrderId = null,
            Amount = request.Amount,
            InstrumentId = instrumentId.Value,
            Price = request.Amount,
            Quantity = 1,
        };

        await _tradeOperationsProducer.Produce(opPayIn);
    }
}
