using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Host.Requests;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.Platform.Host.Controllers;
[Route("orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IOrderExecutionGateway _orderExecutionGateway;

    public OrdersController(
        IOrderExecutionGateway orderExecutionGateway,
        IMediator mediator)
    {
        _orderExecutionGateway = orderExecutionGateway;
        _mediator = mediator;
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
        var mediatorRequest = new ExecuteOrderRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            QtyLots = request.Lots,
            RequestId = Guid.NewGuid(),
        };

        var response = await _mediator.Send(mediatorRequest);
        return Ok(response);
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenPosition(OpenRequest request)
    {
        var mediatorRequest = new OpenPositionRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            QtyLots = request.Lots,
            RequestId = Guid.NewGuid(),
        };

        var response = await _mediator.Send(mediatorRequest);
        return Ok(response);
    }

    [HttpPost("close")]
    public async Task<IActionResult> ClosePosition(CloseRequest request)
    {
        var mediatorRequest = new ClosePositionRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            RequestId = Guid.NewGuid(),
        };

        var response = await _mediator.Send(mediatorRequest);
        return Ok(response);
    }

    [HttpPost("reverse")]
    public async Task<IActionResult> RevertPosition(ReverseRequest request)
    {
        var mediatorRequest = new OpenPositionRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            RequestId = Guid.NewGuid(),
        };

        var response = await _mediator.Send(mediatorRequest);
        return Ok(response);
    }

    [HttpPost("signal")]
    public async Task<IActionResult> PorocessSignal(SignalRequest request)
    {
        var mediatorRequest = new TradingSignalRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioIdentity = new PortfolioIdentity(request.AccountId, request.SubAccountId),
            QtyLots = request.Lots,
            RequestId = Guid.NewGuid(),
        };

        var response = await _mediator.Send(mediatorRequest);
        return Ok(response);
    }
}
