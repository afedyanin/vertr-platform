using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vertr.OrderExecution.Application;
using Vertr.OrderExecution.Contracts.Commands;
using Vertr.OrderExecution.Contracts.Requests;
using Vertr.Platform.Common.Mediator;

namespace Vertr.OrderExecution.WebApi;

[Route("api/positions")]
[ApiController]
public class PositionsController : ControllerBase
{
    private readonly IMediator _mediator;

    private readonly OrderExecutionSettings _orderExecutionSettings;

    public PositionsController(
        IMediator mediator,
        IOptions<OrderExecutionSettings> options)
    {
        _orderExecutionSettings = options.Value;
        _mediator = mediator;
    }

    [HttpPost("execute-order")]
    public async Task<IActionResult> ExecuteOrder(ExecuteRequest request)
    {
        var command = new ExecuteOrderCommand
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

    [HttpPost("open")]
    public async Task<IActionResult> OpenPosition(OpenRequest request)
    {
        var command = new OpenPositionCommand
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
        var command = new ClosePositionCommand
        {
            InstrumentId = request.InstrumentId,
            PortfolioId = request.PortfolioId,
            Price = request.Price,
            RequestId = Guid.NewGuid(),
            CreatedAt = request.Date,
        };

        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("reverse")]
    public async Task<IActionResult> RevertPosition(ReverseRequest request)
    {
        var command = new ReversePositionCommand
        {
            InstrumentId = request.InstrumentId,
            PortfolioId = request.PortfolioId,
            Price = request.Price,
            RequestId = Guid.NewGuid(),
            CreatedAt = request.Date,
        };

        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("signal")]
    public async Task<IActionResult> PorocessSignal(SignalRequest request)
    {
        var command = new TradingSignalCommand
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

    [HttpGet("simulated-order-execution")]
    public IActionResult GetOrderExecutionMode()
    {
        return Ok(_orderExecutionSettings.SimulatedExecution);
    }
}
