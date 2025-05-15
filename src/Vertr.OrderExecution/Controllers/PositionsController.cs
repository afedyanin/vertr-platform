using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Controllers;
[Route("positions")]
[ApiController]

public class PositionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PositionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenPosition(OpenPositionRequest positionRequest)
    {
        var request = new Application.Commands.OpenPositionRequest();
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("close")]
    public async Task<IActionResult> ClosePosition(ClosePositionRequest positionRequest)
    {
        var request = new Application.Commands.ClosePositionRequest();
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("revert")]
    public async Task<IActionResult> RevertPosition(RevertPositionRequest positionRequest)
    {
        var request = new Application.Commands.RevertPositionRequest();
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}
