using Microsoft.AspNetCore.Mvc;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;

[Route("portfolio")]
[ApiController]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IPortfolioGateway _portfolioGateway;

    public PortfolioController(
        IPortfolioGateway portfolioGateway,
        IPortfolioRepository portfolioRepository)
    {
        _portfolioGateway = portfolioGateway;
        _portfolioRepository = portfolioRepository;
    }

    [HttpGet("{accountId}")]
    public IActionResult GetPortfolio(string accountId, Guid? bookId = null)
    {
        var identity = new PortfolioIdentity(accountId, bookId);
        var portfolio = _portfolioRepository.GetPortfolio(identity);
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

    [HttpGet("gateway-portfolio/{accountId}")]
    public async Task<IActionResult> GetGatewayPortfolio(string accountId)
    {
        var portfolio = await _portfolioGateway.GetPortfolio(accountId);
        return Ok(portfolio);
    }

}
