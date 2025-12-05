using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vertr.TinvestGateway.Abstractions;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Portfolios;

namespace Vertr.TinvestGateway.Host.Controllers;

[Route("api/tinvest")]
[ApiController]
public class TinvestGatewayController : ControllerBase
{
    private readonly IMarketDataGateway _marketDataGayeway;
    private readonly IOrderExecutionGateway _orderExecutionGateway;
    private readonly IPortfolioGateway _portfolioGateway;
    private readonly TinvestSettings _tinvestSettings;

    public TinvestGatewayController(
        IMarketDataGateway marketDataGateway,
        IOrderExecutionGateway orderExecutionGateway,
        IPortfolioGateway portfolioGateway,
        IOptions<TinvestSettings> tinvestSettings)
    {
        _marketDataGayeway = marketDataGateway;
        _orderExecutionGateway = orderExecutionGateway;
        _portfolioGateway = portfolioGateway;
        _tinvestSettings = tinvestSettings.Value;
    }

    [HttpGet("instrument-by-ticker/{classCode}/{ticker}")]
    public async Task<IActionResult> GetInstrumentByTicker(string classCode, string ticker)
    {
        var instrument = await _marketDataGayeway.GetInstrumentBySymbol(classCode, ticker);
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
        var accountId = _tinvestSettings.AccountId;
        var orderState = await _orderExecutionGateway.GetOrderState(accountId, orderId);
        return Ok(orderState);
    }

    [HttpDelete("order/{orderId}")]
    public async Task<IActionResult> CancelOrder(string orderId)
    {
        var accountId = _tinvestSettings.AccountId;
        var date = await _orderExecutionGateway.CancelOrder(accountId, orderId);
        return Ok(date);
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _portfolioGateway.GetAccounts();
        return Ok(accounts);
    }

    [HttpGet("sandbox-accounts")]
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

    [HttpPut("sandbox-account/{accountId}/pay-in")]
    public async Task<IActionResult> PayIn(string accountId, decimal amount)
    {
        var money = new Money(amount, "rub");
        var balance = await _portfolioGateway.PayIn(accountId, money);
        return Ok(balance);
    }

    [HttpDelete("sandbox-account/{accountId}")]
    public async Task<IActionResult> CloseAccount(string accountId)
    {
        await _portfolioGateway.CloseSandboxAccount(accountId);
        return Ok();
    }

    [HttpGet("account/{accountId}/portfolio")]
    public async Task<IActionResult> GetGatewayPortfolio(string accountId)
    {
        var portfolio = await _portfolioGateway.GetPortfolio(accountId);
        return Ok(portfolio);
    }

    [HttpGet("account/{accountId}/operations")]
    public async Task<IActionResult> GetGatewayOperations(string accountId, DateTime? from = null, DateTime? to = null)
    {
        var operations = await _portfolioGateway.GetOperations(accountId, from ?? DateTime.UtcNow.AddDays(-10), to ?? DateTime.UtcNow);
        return Ok(operations);
    }
}