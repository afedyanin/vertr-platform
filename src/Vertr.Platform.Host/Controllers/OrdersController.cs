using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;

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
    public async Task<IActionResult> ExecuteOrder(ExecuteOrderRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenPosition(OpenPositionRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("close")]
    public async Task<IActionResult> ClosePosition(ClosePositionRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("reverse")]
    public async Task<IActionResult> RevertPosition(ReversePositionRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("signal")]
    public async Task<IActionResult> PorocessSignal(TradingSignalRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}
