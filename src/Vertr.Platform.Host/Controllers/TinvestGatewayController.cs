using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Application;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;

[Route("api/tinvest")]
[ApiController]
public class TinvestGatewayController : ControllerBase
{
    private readonly IMarketDataGateway _marketDataGayeway;
    private readonly IOrderExecutionGateway _orderExecutionGateway;
    private readonly OrderExecutionSettings _orderExecutionSettings;

    public TinvestGatewayController(
        IMarketDataGateway marketDataGayeway,
        IOptions<OrderExecutionSettings> options,
        IOrderExecutionGateway orderExecutionGateway)
    {
        _marketDataGayeway = marketDataGayeway;
        _orderExecutionGateway = orderExecutionGateway;
        _orderExecutionSettings = options.Value;
    }

    [HttpGet("instrument-by-ticker/{classCode}/{ticker}")]
    public async Task<IActionResult> GetInstrumentByTicker(string classCode, string ticker)
    {
        var symbol = new Symbol(classCode, ticker);
        var instrument = await _marketDataGayeway.GetInstrumentBySymbol(symbol);
        return Ok(instrument);
    }

    [HttpGet("instrument-by-id/{instrumentId:guid}")]
    public async Task<IActionResult> GetInstrumentById(Guid instrumentId)
    {
        var instrument = await _marketDataGayeway.GetInstrumentById(instrumentId);
        return Ok(instrument);
    }

    [HttpGet("instrument-find/{query}")]
    public async Task<IActionResult> FindInstrument(string query)
    {
        var instruments = await _marketDataGayeway.FindInstrument(query);
        return Ok(instruments);
    }

    [HttpGet("candles/{instrumentId}")]
    public async Task<IActionResult> GetCandles(Guid instrumentId, DateOnly? date = null)
    {
        var candles = await _marketDataGayeway.GetCandles(instrumentId, date);
        return Ok(candles);
    }

    [HttpPost("orders")]
    public async Task<IActionResult> PostOrder(PostOrderRequest request)
    {
        var response = await _orderExecutionGateway.PostOrder(request);
        return Ok(response);
    }

    [HttpGet("order-state/{orderId}")]
    public async Task<IActionResult> GetOrderState(string orderId)
    {
        var accountId = _orderExecutionSettings.AccountId;
        var orderState = await _orderExecutionGateway.GetOrderState(accountId, orderId);
        return Ok(orderState);
    }

    [HttpDelete("order/{orderId}")]
    public async Task<IActionResult> CancelOrder(string orderId)
    {
        var accountId = _orderExecutionSettings.AccountId;
        var date = await _orderExecutionGateway.CancelOrder(accountId, orderId);
        return Ok(date);
    }
}
