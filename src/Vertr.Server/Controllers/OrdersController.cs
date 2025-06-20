using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts.Requests;

namespace Vertr.Server.Controllers;

[Route("positions")]
[ApiController]

public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
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

    [HttpPost()]
    public async Task<IActionResult> Porocess(TradingSignalRequest request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}
