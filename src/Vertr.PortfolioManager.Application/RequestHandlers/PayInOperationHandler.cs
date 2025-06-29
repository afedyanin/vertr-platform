using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;

internal class PayInOperationHandler : IRequestHandler<PayInOperationRequest>
{
    private readonly ITradeOperationRepository _operationEventRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ITradeOperationService _tadeOperationService;
    private readonly IStaticMarketDataProvider _staticMarketDataProvider;
    private readonly ILogger<PayInOperationHandler> _logger;

    public PayInOperationHandler(
        ITradeOperationRepository operationEventRepository,
        IPortfolioRepository portfolioRepository,
        ITradeOperationService tadeOperationService,
        IStaticMarketDataProvider staticMarketDataProvider,
        ILogger<PayInOperationHandler> logger)
    {
        _operationEventRepository = operationEventRepository;
        _portfolioRepository = portfolioRepository;
        _tadeOperationService = tadeOperationService;
        _staticMarketDataProvider = staticMarketDataProvider;
        _logger = logger;
    }

    public async Task Handle(PayInOperationRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Pay in operation received.");

        var instrumentId = await _staticMarketDataProvider.GetCurrencyId(request.Amount.Currency);

        if (instrumentId == null)
        {
            throw new InvalidOperationException($"Unknown currecncy={request.Amount.Currency}");
        }

        var opPayIn = new TradeOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = TradeOperationType.Input,
            AccountId = request.AccountId,
            SubAccountId = request.SubAccountId,
            OrderId = null,
            Amount = request.Amount,
            InstrumentId = instrumentId.Value,
            Price = request.Amount,
            Quantity = 1,
            Message = "Внесение средств на счет"
        };

        var saved = await _operationEventRepository.Save([opPayIn]);

        if (!saved)
        {
            throw new InvalidOperationException($"Cannot save pay in operation for AccountId={request.AccountId}");
        }

        var portfolio = await _tadeOperationService.ApplyOperation(opPayIn);
        _portfolioRepository.Save(portfolio);
    }
}
