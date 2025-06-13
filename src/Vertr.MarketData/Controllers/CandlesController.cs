using Microsoft.AspNetCore.Mvc;
using Vertr.MarketData.Application.Abstractions;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Controllers;

[Route("candles")]
[ApiController]
public class CandlesController : ControllerBase
{
    private readonly ICandlesRepository _repository;

    public CandlesController(ICandlesRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> GetByQuery(CandlesQuery query)
    {
        var candles = await _repository.GetByQuery(query);

        if (candles == null)
        {
            return NotFound();
        }

        return Ok(candles);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        string symbol,
        CandleInterval interval,
        CandleSource source)
    {
        //var deleted = await _repository.Delete(symbol, interval, source);

        return Ok();
    }
}
