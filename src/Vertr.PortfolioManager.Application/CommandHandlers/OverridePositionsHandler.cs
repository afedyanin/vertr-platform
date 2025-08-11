using MediatR;
using Vertr.MarketData.Contracts.Interfaces.old;
using Vertr.Platform.Common.Channels;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Commands;

namespace Vertr.PortfolioManager.Application.CommandHandlers;

internal class OverridePositionsHandler : IRequestHandler<OverridePositionsCommand>
{
    private readonly IDataProducer<TradeOperation> _tradeOperationsProducer;
    private readonly ICurrencyRepository _currencyRepository;

    public OverridePositionsHandler(
        IDataProducer<TradeOperation> tradeOperationsProducer,
        ICurrencyRepository currencyRepository)
    {
        _tradeOperationsProducer = tradeOperationsProducer;
        _currencyRepository = currencyRepository;
    }

    public async Task Handle(OverridePositionsCommand request, CancellationToken cancellationToken)
    {
        var operations = await CreateOperations(request);

        foreach (var operation in operations)
        {
            await _tradeOperationsProducer.Produce(operation, cancellationToken);
        }
    }

    private async Task<TradeOperation[]> CreateOperations(OverridePositionsCommand request)
    {
        var operations = new List<TradeOperation>();

        foreach (var item in request.Overrides)
        {
            var currency = await _currencyRepository.GetInstrumentCurrency(item.InstrumentId);
            var balance = new Money(item.Balance, currency);

            var op = new TradeOperation
            {
                CreatedAt = request.CreatedAt,
                OperationType = TradeOperationType.PositionOverride,
                AccountId = request.AccountId,
                SubAccountId = request.SubAccountId,
                Amount = balance,
                InstrumentId = item.InstrumentId,
                Price = balance,
                Quantity = 1
            };

            operations.Add(op);
        }

        return [.. operations];
    }
}
