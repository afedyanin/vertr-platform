using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.Domain.Ports;
using Vertr.Domain.Repositories;

namespace Vertr.Application.Orders;
internal class ExecuteOrderHandler : IRequestHandler<ExecuteOrderRequest>
{
    private readonly ITinvestGateway _gateway;
    private readonly ITinvestOrdersRepository _repository;
    private readonly ILogger<ExecuteOrderHandler> _logger;

    public ExecuteOrderHandler(
        ITinvestGateway gateway,
        ITinvestOrdersRepository repository,
        ILogger<ExecuteOrderHandler> logger)
    {
        _gateway = gateway;
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(ExecuteOrderRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Posting order to gateway: AccountId={request.PostOrderRequest.AccountId} TradingSignalId={request.TradingSignalId}");

        var response = await _gateway.PostOrder(request.PostOrderRequest);

        response.TradingSignalId = request.TradingSignalId;

        var saved = await _repository.Insert(response, cancellationToken);

        _logger.LogInformation($"{saved} post order response saved. OrderResponseId={response.Id} AccountId={request.PostOrderRequest.AccountId} TradingSignalId={request.TradingSignalId}");
    }
}
