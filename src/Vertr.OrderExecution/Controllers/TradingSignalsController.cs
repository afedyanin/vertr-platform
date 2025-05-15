using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Controllers;
[Route("signals")]
[ApiController]
public class TradingSignalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TradingSignalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost()]
    public async Task<IActionResult> Porocess(TradingSignalRequest signalRequest)
    {
        var request = new Application.Commands.TradingSignalRequest();
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}
