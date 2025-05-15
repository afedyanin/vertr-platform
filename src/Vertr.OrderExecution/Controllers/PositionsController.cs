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
    public async Task<IActionResult> OpenPosition(OpenPositionRequest request)
    {
        var openRequest = new Application.Commands.OpenPositionRequest
        {
            AccountId = request.AccountId,
            RequestId = request.RequestId,
            InstrumentId = request.InstrumentId,
            QtyLots = request.QtyLots,
        };

        var response = await _mediator.Send(openRequest);

        // TODO Convert response to contract
        return Ok(response);
    }

    [HttpPost("close")]
    public async Task<IActionResult> ClosePosition(ClosePositionRequest request)
    {
        var closeRequest = new Application.Commands.ClosePositionRequest
        {
            AccountId = request.AccountId,
            RequestId = request.RequestId,
            InstrumentId = request.InstrumentId,
        };

        var response = await _mediator.Send(closeRequest);

        // TODO Convert response to contract
        return Ok(response);
    }

    [HttpPost("revert")]
    public async Task<IActionResult> RevertPosition(RevertPositionRequest request)
    {
        var revertRequest = new Application.Commands.ReversePositionRequest
        {
            AccountId = request.AccountId,
            RequestId = request.RequestId,
            InstrumentId = request.InstrumentId,
        };
        var response = await _mediator.Send(revertRequest);

        // TODO Convert response to contract
        return Ok(response);
    }
}
