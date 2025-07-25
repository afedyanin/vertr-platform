using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Common.Mediator;
using Vertr.Platform.Host.Requests;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Host.Controllers;
[Route("orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderExecutionGateway _orderExecutionGateway;
    private readonly ICommandHandler<ClosePositionCommand, ExecuteOrderResponse> _closePositionHandler;
    private readonly ICommandHandler<ExecuteOrderCommand, ExecuteOrderResponse> _executeOrderHandler;
    private readonly ICommandHandler<OpenPositionCommand, ExecuteOrderResponse> _openPositionHandler;
    private readonly ICommandHandler<ReversePositionCommand, ExecuteOrderResponse> _reversePositionHandler;
    private readonly ICommandHandler<TradingSignalCommand, ExecuteOrderResponse> _tradingSignalHandler;

    public OrdersController(
        ICommandHandler<ClosePositionCommand, ExecuteOrderResponse> closePositionHandler,
        ICommandHandler<ExecuteOrderCommand, ExecuteOrderResponse> executeOrderHandler,
        ICommandHandler<OpenPositionCommand, ExecuteOrderResponse> openPositionHandler,
        ICommandHandler<ReversePositionCommand, ExecuteOrderResponse> reversePositionHandler,
        ICommandHandler<TradingSignalCommand, ExecuteOrderResponse> tradingSignalHandler,
        IOrderExecutionGateway orderExecutionGateway)
    {
        _orderExecutionGateway = orderExecutionGateway;

        _closePositionHandler = closePositionHandler;
        _executeOrderHandler = executeOrderHandler;
        _openPositionHandler = openPositionHandler;
        _reversePositionHandler = reversePositionHandler;
        _tradingSignalHandler = tradingSignalHandler;
    }

    [HttpPost("order")]
    public async Task<IActionResult> PostOrder(PostOrderRequest request)
    {
        var response = await _orderExecutionGateway.PostOrder(request);
        return Ok(response);
    }

    [HttpGet("order-state/{accountId}/{orderId}")]
    public async Task<IActionResult> GetOrderState(string accountId, string orderId)
    {
        var orderState = await _orderExecutionGateway.GetOrderState(accountId, orderId);
        return Ok(orderState);
    }

    [HttpDelete("order/{accountId}/{orderId}")]
    public async Task<IActionResult> CancelOrder(string accountId, string orderId)
    {
        var date = await _orderExecutionGateway.CancelOrder(accountId, orderId);
        return Ok(date);
    }

    [HttpPost("execute")]
    public async Task<IActionResult> ExecuteOrder(ExecuteRequest request)
    {
        var command = new ExecuteOrderCommand
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            QtyLots = request.Lots,
            RequestId = Guid.NewGuid(),
        };

        var response = await _executeOrderHandler.Handle(command);

        return Ok(response);
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenPosition(OpenRequest request)
    {
        var command = new OpenPositionCommand
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            QtyLots = request.Lots,
            RequestId = Guid.NewGuid(),
        };

        var response = await _openPositionHandler.Handle(command);
        return Ok(response);
    }

    [HttpPost("close")]
    public async Task<IActionResult> ClosePosition(CloseRequest request)
    {
        var command = new ClosePositionCommand
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            RequestId = Guid.NewGuid(),
        };

        var response = await _closePositionHandler.Handle(command);
        return Ok(response);
    }

    [HttpPost("reverse")]
    public async Task<IActionResult> RevertPosition(ReverseRequest request)
    {
        var command = new ReversePositionCommand
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            RequestId = Guid.NewGuid(),
        };

        var response = await _reversePositionHandler.Handle(command);
        return Ok(response);
    }

    [HttpPost("signal")]
    public async Task<IActionResult> PorocessSignal(SignalRequest request)
    {
        var command = new TradingSignalCommand
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            QtyLots = request.Lots,
            RequestId = Guid.NewGuid(),
        };

        var response = await _tradingSignalHandler.Handle(command);
        return Ok(response);
    }
}
