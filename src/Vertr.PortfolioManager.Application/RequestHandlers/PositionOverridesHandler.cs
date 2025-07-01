using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;
internal class PositionOverridesHandler : IRequestHandler<PositionOverridesRequest>
{
    private readonly ITradeOperationRepository _operationEventRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ITradeOperationService _tadeOperationService;
    private readonly IStaticMarketDataProvider _staticMarketDataProvider;
    private readonly ILogger<TradeOperationsHandler> _logger;

    public PositionOverridesHandler(
        ITradeOperationRepository operationEventRepository,
        IPortfolioRepository portfolioRepository,
        ITradeOperationService tadeOperationService,
        IStaticMarketDataProvider staticMarketDataProvider,
        ILogger<TradeOperationsHandler> logger)
    {
        _operationEventRepository = operationEventRepository;
        _portfolioRepository = portfolioRepository;
        _tadeOperationService = tadeOperationService;
        _staticMarketDataProvider = staticMarketDataProvider;
        _logger = logger;
    }


    public async Task Handle(PositionOverridesRequest request, CancellationToken cancellationToken)
    {
        var operations = await CreateOperations(request);

        _logger.LogInformation($"Position overrides received.");

        if (operations.Length <= 0)
        {
            _logger.LogError("No operations from PositionOverridesRequest");
            return;
        }

        await SaveOperations(operations);

        foreach (var operation in operations)
        {
            var portfolio = await _tadeOperationService.ApplyOperation(operation);
            _portfolioRepository.Save(portfolio);
        }
    }

    private async Task<TradeOperation[]> CreateOperations(PositionOverridesRequest request)
    {
        var operations = new List<TradeOperation>();

        foreach (var item in request.Overrides)
        {
            var currency = await _staticMarketDataProvider.GetInstrumentCurrency(item.InstrumentId);
            var balance = new Money(item.Balance, currency);

            var op = new TradeOperation
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
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

    private async Task SaveOperations(TradeOperation[] operations)
    {
        var saved = await _operationEventRepository.Save(operations);

        if (!saved)
        {
            throw new InvalidOperationException("Cannot save position override operations.");
        }
    }
}
