using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Contracts.Commands;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Mediator;

namespace Vertr.MarketData.WebApi;

[Route("api/candles")]
[ApiController]
public class CandlesController : ControllerBase
{
    private readonly ICandlesRepository _candlesRepository;
    private readonly IMediator _mediator;

    public CandlesController(
        IMediator mediator,
        ICandlesRepository candlesRepository)
    {
        _mediator = mediator;
        _candlesRepository = candlesRepository;
    }

    [HttpGet("{instrumentId:guid}")]
    public async Task<IActionResult> GetById(Guid instrumentId, int limit = 500)
    {
        var candles = await _candlesRepository.Get(instrumentId, limit);

        return Ok(candles);
    }

    [HttpPost("load-intraday")]
    public async Task<IActionResult> LoadIntraday()
    {
        var request = new LoadIntradayCandlesRequest();
        await _mediator.Send(request);
        return Ok();
    }
}
