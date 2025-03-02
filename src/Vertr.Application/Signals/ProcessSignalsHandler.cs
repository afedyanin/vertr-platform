using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.Application.Orders;
using Vertr.Domain;
using Vertr.Domain.Enums;
using Vertr.Domain.Repositories;
using Vertr.Domain.Settings;

namespace Vertr.Application.Signals;
internal class ProcessSignalsHandler : IRequestHandler<ProcessSignalsRequest>
{
    private readonly ITradingSignalsRepository _signalsRepository;
    private readonly ITinvestOrdersRepository _tinvestOrdersRepository;
    private readonly AccountStrategySettings _accountStrategySettings;
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessSignalsHandler> _logger;

    public ProcessSignalsHandler(
        ITradingSignalsRepository signalsRepository,
        ITinvestOrdersRepository tinvestOrdersRepository,
        IMediator mediator,
        IOptions<AccountStrategySettings> options,
        ILogger<ProcessSignalsHandler> logger)
    {
        _signalsRepository = signalsRepository;
        _tinvestOrdersRepository = tinvestOrdersRepository;
        _accountStrategySettings = options.Value;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(ProcessSignalsRequest request, CancellationToken cancellationToken)
    {
        // TODO: Implement error handling
        // TODO: Implement parallel tasks

        foreach (var accountId in request.Accounts)
        {
            if (!_accountStrategySettings.SignalMappings.TryGetValue(accountId, out var strategySettingsList))
            {
                _logger.LogInformation($"Trading strategy not found for accountId={accountId}. Skipping.");
                continue;
            }

            foreach (var strategySettings in strategySettingsList)
            {
                await HandleInternal(accountId, strategySettings, cancellationToken);
            }
        }
    }

    private async Task HandleInternal(
        string accountId,
        StrategySettings strategySettings,
        CancellationToken cancellationToken)
    {
        var lastSignal = await _signalsRepository.GetLast(strategySettings, cancellationToken);

        if (lastSignal == null)
        {
            _logger.LogDebug($"No new signals found for {strategySettings}");
            return;
        }

        if (lastSignal.Action == TradeAction.Hold)
        {
            _logger.LogDebug($"Last signal action == Hold  for {strategySettings}. Nothing to do.");
            return;
        }

        var order = await _tinvestOrdersRepository.GetByTradingSignal(accountId, lastSignal.Id);

        if (order != null)
        {
            _logger.LogDebug($"Last signal for {strategySettings} is already processed. OrderId={order.Id}");
            return;
        }

        var approvalRequest = new OrderApprovementRequest
        {
            AccountId = accountId,
            Signal = lastSignal,
        };

        var approvalResponse = await _mediator.Send(approvalRequest);

        if (approvalResponse.ApprovedQuantityLots == 0)
        {
            _logger.LogDebug($"No approved quantity received for {strategySettings} SignalId={lastSignal.Id}");
            return;
        }

        var postOrderRequest = new PostOrderRequest
        {
            AccountId = accountId,
            Symbol = strategySettings.Symbol,
            OrderDirection = approvalResponse.OrderDirection,
            QuantityLots = approvalResponse.ApprovedQuantityLots,
            OrderType = OrderType.Market,
            Price = decimal.Zero,
            PriceType = PriceType.Unspecified,
            TimeInForceType = TimeInForceType.Unspecified,
            RequestId = Guid.NewGuid(),
        };

        // So we should post order
        var executeOrderRequest = new ExecuteOrderRequest
        {
            TradingSignalId = lastSignal.Id,
            PostOrderRequest = postOrderRequest,
        };

        var executionResponse = await _mediator.Send(executeOrderRequest);
        _logger.LogInformation($"New order posted for {strategySettings} SignalId={lastSignal.Id} OrderResponseId={executionResponse.OrderResponseId}");
    }
}
