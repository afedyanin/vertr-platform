using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.Platform.Common.Mediator;
using Vertr.Platform.Host.Requests;

namespace Vertr.Platform.Host.Controllers;
[Route("api/positions")]
[ApiController]
public class PositionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PositionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("execute-order")]
    public async Task<IActionResult> ExecuteOrder(ExecuteRequest request)
    {
        var command = new ExecuteOrderRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioId = request.PortfolioId,
            QtyLots = request.Lots,
            Price = request.Price,
            RequestId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
        };

        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenPosition(OpenRequest request)
    {
        var command = new OpenPositionRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioId = request.PortfolioId,
            QtyLots = request.Lots,
            Price = request.Price,
            RequestId = Guid.NewGuid(),
            CreatedAt = request.Date,
        };

        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("close")]
    public async Task<IActionResult> ClosePosition(CloseRequest request)
    {
        var command = new ClosePositionRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioId = request.PortfolioId,
            Price = request.Price,
            RequestId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
        };

        var response = await _mediator.Send (command);
        return Ok(response);
    }

    [HttpPost("reverse")]
    public async Task<IActionResult> RevertPosition(ReverseRequest request)
    {
        var command = new ReversePositionRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioId = request.PortfolioId,
            Price = request.Price,
            RequestId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
        };

        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("signal")]
    public async Task<IActionResult> PorocessSignal(SignalRequest request)
    {
        var command = new TradingSignalRequest
        {
            InstrumentId = request.InstrumentId,
            PortfolioId = request.PortfolioId,
            QtyLots = request.Lots,
            Price = request.Price,
            RequestId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
        };

        var response = await _mediator.Send(command);
        return Ok(response);
    }
}
