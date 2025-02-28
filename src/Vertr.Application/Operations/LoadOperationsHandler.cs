using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.Domain.Ports;
using Vertr.Domain.Repositories;

namespace Vertr.Application.Operations;

internal class LoadOperationsHandler : IRequestHandler<LoadOperationsRequest>
{
    private readonly ITinvestOperationsRepository _repository;
    private readonly ITinvestGateway _gateway;

    private readonly ILogger<LoadOperationsHandler> _logger;

    public LoadOperationsHandler(
        ITinvestOperationsRepository repository,
        ITinvestGateway gateway,
        ILogger<LoadOperationsHandler> logger)
    {
        _repository = repository;
        _gateway = gateway;
        _logger = logger;
    }


    public async Task Handle(LoadOperationsRequest request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var accountId in request.Accounts)
        {
            tasks.Add(LoadOperations(accountId, cancellationToken));
        }

        await Task.WhenAll(tasks);

        _logger.LogDebug($"Loading T-Invest operations for {request.Accounts.Count()} accounts completed.");
    }

    internal async Task LoadOperations(string accountId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(accountId))
        {
            throw new ArgumentNullException(nameof(accountId));
        }

        try
        {
            _logger.LogDebug($"Loading operations for {accountId}");

            // TODO: get last operation from DB
            // TODO: use time interval to load only latest operations

            var operations = await _gateway.GetOperations(accountId);

            if (operations == null || !operations.Any())
            {
                _logger.LogDebug($"No new operations found for {accountId}.");
                return;
            }

            // TODO: implement InsertMany
            // TODO: check if already exists
            var count = 0;
            foreach (var operation in operations)
            {
                var insetred = await _repository.Insert(operation, cancellationToken);
                count += insetred;
            }

            _logger.LogDebug($"{count} operations inserted/updated for {accountId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "T-Invest operations loading error.");
        }
    }
}
