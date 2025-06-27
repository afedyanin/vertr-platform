using Microsoft.AspNetCore.Mvc;
using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;

[Route("portfolio")]
[ApiController]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioManager _portfolioManager;
    private readonly IPortfolioGateway _portfolioGateway;

    public PortfolioController(
        IPortfolioGateway portfolioGateway,
        IPortfolioManager portfolioManager)
    {
        _portfolioGateway = portfolioGateway;
        _portfolioManager = portfolioManager;
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetLast(string accountId, Guid? bookId = null)
    {
        var identity = new PortfolioIdentity(accountId, bookId);
        var portfolio = await _portfolioManager.GetPortfolio(identity);
        return Ok(portfolio);
    }

    [HttpGet("sandbox-accounts")]
    public async Task<IActionResult> GetSandboxAccounts()
    {
        var accounts = await _portfolioGateway.GetSandboxAccounts();
        return Ok(accounts);
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _portfolioGateway.GetAccounts();
        return Ok(accounts);
    }

    [HttpPost("sandbox-account")]
    public async Task<IActionResult> CreateAccount(string accountName)
    {
        var accountId = await _portfolioGateway.CreateSandboxAccount(accountName);
        return Ok(accountId);
    }

    [HttpPut("sandbox-account/{accountId}")]
    public async Task<IActionResult> PayIn(string accountId, decimal amount, string currency = "RUB")
    {
        var money = new Money(amount, currency);
        var balance = await _portfolioGateway.PayIn(accountId, money);
        return Ok(balance);
    }

    [HttpDelete("sandbox-account/{accountId}")]
    public async Task<IActionResult> CloseAccount(string accountId)
    {
        await _portfolioGateway.CloseSandboxAccount(accountId);
        return Ok();
    }

    [HttpGet("portfolio/{accountId}")]
    public async Task<IActionResult> GetPortfolio(string accountId)
    {
        var portfolio = await _portfolioGateway.GetPortfolio(accountId);
        return Ok(portfolio);
    }

}
