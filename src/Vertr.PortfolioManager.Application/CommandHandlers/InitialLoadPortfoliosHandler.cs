using Vertr.Platform.Common.Channels;
using Vertr.Platform.Common.Mediator;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Commands;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application.CommandHandlers;

internal class InitialLoadPortfoliosHandler : ICommandHandler<InitialLoadPortfoliosCommand>
{
    private readonly ITradeOperationRepository _tradeOperationRepository;
    private readonly IDataProducer<TradeOperation> _tradeOperationsProducer;

    public InitialLoadPortfoliosHandler(
        IDataProducer<TradeOperation> tradeOperationsProducer,
        ITradeOperationRepository tradeOperationRepository)
    {
        _tradeOperationRepository = tradeOperationRepository;
        _tradeOperationsProducer = tradeOperationsProducer;
    }

    public async Task Handle(InitialLoadPortfoliosCommand request, CancellationToken cancellationToken)
    {
        var allOperations = await _tradeOperationRepository.GetAll();

        foreach (var operation in allOperations)
        {
            await _tradeOperationsProducer.Produce(operation);
        }
    }
}
