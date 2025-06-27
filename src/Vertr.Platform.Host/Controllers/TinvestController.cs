using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;

[Route("tinvest")]
[ApiController]
public class TinvestController : ControllerBase
{
    private readonly ITinvestGatewayAccounts _tinvestGatewayAccounts;
    private readonly IOrderExecutionGateway _tinvestGatewayOrders;

    public TinvestController(
        ITinvestGatewayAccounts tinvestGatewayAccounts,
        IOrderExecutionGateway tinvestGatewayOrders
        )
    {
        _tinvestGatewayAccounts = tinvestGatewayAccounts;
        _tinvestGatewayOrders = tinvestGatewayOrders;
    }

    [HttpGet("sandbox-accounts")]
    public async Task<IActionResult> GetSandboxAccounts()
    {
        var accounts = await _tinvestGatewayAccounts.GetSandboxAccounts();
        return Ok(accounts);
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _tinvestGatewayAccounts.GetAccounts();
        return Ok(accounts);
    }

    [HttpPost("sandbox-account")]
    public async Task<IActionResult> CreateAccount(string accountName)
    {
        var accountId = await _tinvestGatewayAccounts.CreateSandboxAccount(accountName);
        return Ok(accountId);
    }

    [HttpPut("sandbox-account/{accountId}")]
    public async Task<IActionResult> PayIn(string accountId, decimal amount, string currency = "RUB")
    {
        var money = new Money(amount, currency);
        var balance = await _tinvestGatewayAccounts.PayIn(accountId, money);
        return Ok(balance);
    }

    [HttpDelete("sandbox-account/{accountId}")]
    public async Task<IActionResult> CloseAccount(string accountId)
    {
        await _tinvestGatewayAccounts.CloseSandboxAccount(accountId);
        return Ok();
    }

    [HttpGet("operations/{accountId}")]
    public async Task<IActionResult> GetOperations(string accountId, DateTime? from = null, DateTime? to = null)
    {
        var operations = await _tinvestGatewayAccounts.GetOperations(accountId, from, to);
        return Ok(operations);
    }

    [HttpGet("positions/{accountId}")]
    public async Task<IActionResult> GetPositions(string accountId)
    {
        var positions = await _tinvestGatewayAccounts.GetPositions(accountId);
        return Ok(positions);
    }

    [HttpGet("portfolio/{accountId}")]
    public async Task<IActionResult> GetPortfolio(string accountId)
    {
        var portfolio = await _tinvestGatewayAccounts.GetPortfolio(accountId);
        return Ok(portfolio);
    }

    [HttpPost("order")]
    public async Task<IActionResult> PostOrder(PostOrderRequest request)
    {
        var response = await _tinvestGatewayOrders.PostOrder(request);
        return Ok(response);
    }

    [HttpGet("order-state/{accountId}/{orderId}")]
    public async Task<IActionResult> GetOrderState(string accountId, string orderId)
    {
        var orderState = await _tinvestGatewayOrders.GetOrderState(accountId, orderId);
        return Ok(orderState);
    }

    [HttpDelete("order/{accountId}/{orderId}")]
    public async Task<IActionResult> CancelOrder(string accountId, string orderId)
    {
        var date = await _tinvestGatewayOrders.CancelOrder(accountId, orderId);
        return Ok(date);
    }
}
