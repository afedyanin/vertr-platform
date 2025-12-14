using Microsoft.AspNetCore.Mvc;
using Vertr.Common.Contracts;
using Vertr.Common.Contracts.Converters;
using Vertr.Common.DataAccess.Repositories;

namespace Vertr.TinvestGateway.Host.Controllers;

[Route("api/candles")]
[ApiController]
public class CandlesController : ControllerBase
{
    private readonly ICandlestickRepository _candlestickRepository;

    public CandlesController(ICandlestickRepository candlestickRepository)
    {
        _candlestickRepository = candlestickRepository;
    }

    [HttpGet("{instrumentId:guid}")]
    public async Task<Candle[]> GetCandles(Guid instrumentId, [FromQuery] long maxItems = -1)
    {
        var candlesticks = await _candlestickRepository.GetLast(instrumentId, maxItems);
        return candlesticks?.ToCandles(instrumentId) ?? [];
    }
}
