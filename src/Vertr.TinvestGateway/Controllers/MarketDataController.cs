using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Controllers;
[Route("marketdata")]
[ApiController]
public class MarketDataController : TinvestControllerBase
{
    public MarketDataController(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient) : base(options, investApiClient)
    {
    }

    [HttpGet("candles")]
    public async Task<IActionResult> GetCandles(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        int? limit)
    {
        var instrumentId = Settings.GetSymbolId(symbol);

        if (string.IsNullOrEmpty(instrumentId))
        {
            return NotFound();
        }

        var request = new Tinkoff.InvestApi.V1.GetCandlesRequest
        {
            From = Timestamp.FromDateTime(from.ToUniversalTime()),
            To = Timestamp.FromDateTime(to.ToUniversalTime()),
            InstrumentId = instrumentId,
            Interval = interval.Convert(),
        };

        if (limit.HasValue)
        {
            request.Limit = limit.Value;
        }

        var response = await InvestApiClient.MarketData.GetCandlesAsync(request);
        var candles = response.Candles.ToArray().Convert(symbol, interval);

        return Ok(candles);
    }
}
