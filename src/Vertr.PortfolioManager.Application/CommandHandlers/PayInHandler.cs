using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common;
using Vertr.Platform.Common.Channels;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Commands;

namespace Vertr.PortfolioManager.Application.CommandHandlers;

internal class PayInHandler : ICommandHandler<PayInCommand>
{
    private readonly IDataProducer<TradeOperation> _tradeOperationsProducer;

    private readonly IMarketInstrumentRepository _staticMarketDataProvider;
    private readonly ILogger<PayInHandler> _logger;

    public PayInHandler(
        IDataProducer<TradeOperation> tradeOperationsProducer,
        IMarketInstrumentRepository staticMarketDataProvider,
        ILogger<PayInHandler> logger)
    {
        _tradeOperationsProducer = tradeOperationsProducer;
        _staticMarketDataProvider = staticMarketDataProvider;
        _logger = logger;
    }

    public async Task Handle(PayInCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Pay in operation received.");

        var instrumentId = _staticMarketDataProvider.GetCurrencyId(request.Amount.Currency);

        if (instrumentId == null)
        {
            throw new InvalidOperationException($"Unknown currecncy={request.Amount.Currency}");
        }

        var opPayIn = new TradeOperation
        {
            CreatedAt = DateTime.UtcNow,
            OperationType = TradeOperationType.Input,
            AccountId = request.AccountId,
            SubAccountId = request.SubAccountId,
            OrderId = null,
            Amount = request.Amount,
            InstrumentId = instrumentId.Value,
            Price = request.Amount,
            Quantity = 1,
        };

        await _tradeOperationsProducer.Produce(opPayIn);
    }
}
