using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Converters;

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
            RequestId = request.RequestId,
            InstrumentId = request.InstrumentId,
            QtyLots = request.QtyLots,
            PortfolioId = request.PortfolioId,
        };

        var response = await _mediator.Send(openRequest);
        return Ok(response.Convert());
    }

    [HttpPost("close")]
    public async Task<IActionResult> ClosePosition(ClosePositionRequest request)
    {
        var closeRequest = new Application.Commands.ClosePositionRequest
        {
            RequestId = request.RequestId,
            InstrumentId = request.InstrumentId,
            PortfolioId = request.PortfolioId,
        };

        var response = await _mediator.Send(closeRequest);
        return Ok(response.Convert());
    }

    [HttpPost("revert")]
    public async Task<IActionResult> RevertPosition(RevertPositionRequest request)
    {
        var revertRequest = new Application.Commands.ReversePositionRequest
        {
            RequestId = request.RequestId,
            InstrumentId = request.InstrumentId,
            PortfolioId = request.PortfolioId,
        };

        var response = await _mediator.Send(revertRequest);
        return Ok(response.Convert());
    }
}
