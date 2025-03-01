using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.Domain.Settings;

namespace Vertr.Application.Signals;
internal class ProcessSignalsHandler : IRequestHandler<ProcessSignalsRequest>
{
    private readonly AccountStrategySettings _accountStrategySettings;
    private readonly ILogger<ProcessSignalsHandler> _logger;

    public ProcessSignalsHandler(
        IOptions<AccountStrategySettings> options,
        ILogger<ProcessSignalsHandler> logger)
    {
        _accountStrategySettings = options.Value;
        _logger = logger;
    }

    public async Task Handle(ProcessSignalsRequest request, CancellationToken cancellationToken)
    {
        // TODO: Implement error handling

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
        // Get last signals from db by strategy, symbol, interval
        // For each signal do:
        // - if isHoldAction - skip
        // - get matching accounts
        // - for each account do:
        // - - if is already processed for account - skip
        // - - getAprovedQty
        // - - postOrer
    }
}
