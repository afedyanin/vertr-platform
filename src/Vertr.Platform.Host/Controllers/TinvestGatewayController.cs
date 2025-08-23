using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Application;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.TinvestGateway;

namespace Vertr.Platform.Host.Controllers;

[Route("api/tinvest")]
[ApiController]
public class TinvestGatewayController : ControllerBase
{
    private readonly IMarketDataGateway _marketDataGayeway;
    private readonly IOrderExecutionGateway _orderExecutionGateway;
    private readonly IPortfolioGateway _portfolioGateway;
    private readonly OrderExecutionSettings _orderExecutionSettings;
    private readonly TinvestSettings _tinvestSettings;

    public TinvestGatewayController(
        IMarketDataGateway marketDataGayeway,
        IOptions<OrderExecutionSettings> orderOptions,
        IOptions<TinvestSettings> tinvestOptions,
        IOrderExecutionGateway orderExecutionGateway,
        IPortfolioGateway portfolioGateway)
    {
        _marketDataGayeway = marketDataGayeway;
        _orderExecutionGateway = orderExecutionGateway;
        _portfolioGateway = portfolioGateway;
        _orderExecutionSettings = orderOptions.Value;
        _tinvestSettings = tinvestOptions.Value;
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

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _portfolioGateway.GetAccounts();
        return Ok(accounts);
    }

    [HttpGet("sandbox-account")]
    public async Task<IActionResult> GetSandboxAccounts()
    {
        var accounts = await _portfolioGateway.GetSandboxAccounts();
        return Ok(accounts);
    }

    [HttpPost("sandbox-account")]
    public async Task<IActionResult> CreateAccount(string accountName)
    {
        var accountId = await _portfolioGateway.CreateSandboxAccount(accountName);
        return Ok(accountId);
    }

    [HttpDelete("sandbox-account")]
    public async Task<IActionResult> CloseAccount(string? accountId)
    {
        var accId = accountId ?? _tinvestSettings.AccountId;
        await _portfolioGateway.CloseSandboxAccount(accId);
        return Ok();
    }

    [HttpGet("gateway-portfolio")]
    public async Task<IActionResult> GetGatewayPortfolio(string? accountId)
    {
        var accId = accountId ?? _tinvestSettings.AccountId;
        var portfolio = await _portfolioGateway.GetPortfolio(accId);
        return Ok(portfolio);
    }

    [HttpGet("gateway-operations")]
    public async Task<IActionResult> GetGatewayOperations(DateTime from, DateTime to, string? accountId)
    {
        var accId = accountId ?? _tinvestSettings.AccountId;
        var operations = await _portfolioGateway.GetOperations(accId, from, to);
        return Ok(operations);
    }
}
