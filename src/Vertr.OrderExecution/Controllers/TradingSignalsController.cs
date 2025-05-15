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
    public async Task<IActionResult> Porocess(TradingSignalRequest request)
    {
        var signalRequest = new Application.Commands.TradingSignalRequest
        {
            RequestId = request.RequestId,
            AccountId = request.AccountId,
            InstrumentId = request.InstrumentId,
            QtyLots = request.QtyLots,
        };

        var response = await _mediator.Send(signalRequest);

        // TODO Convert response to contract
        return Ok(response);
    }
}
