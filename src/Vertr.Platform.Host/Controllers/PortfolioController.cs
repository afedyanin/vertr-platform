using Microsoft.AspNetCore.Mvc;
using Vertr.PortfolioManager.Application;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Application.Repositories;
using Microsoft.Extensions.Options;
using Vertr.PortfolioManager.Application.Services;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;

[Route("portfolio")]
[ApiController]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IPortfolioGateway _portfolioGateway;
    private readonly IStaticMarketDataProvider _staticMarketDataProvider;

    public PortfolioController(
        IPortfolioGateway portfolioGateway,
        IPortfolioRepository portfolioRepository,
        IStaticMarketDataProvider staticMarketDataProvider)
    {
        _portfolioGateway = portfolioGateway;
        _portfolioRepository = portfolioRepository;
        _staticMarketDataProvider = staticMarketDataProvider;
    }

    [HttpGet("{accountId}")]
    public Task<IActionResult> GetPortfolio(string accountId, Guid? subAccountId = null)
    {
        var identity = new PortfolioIdentity(accountId, subAccountId);
        var portfolio = _portfolioRepository.GetPortfolio(identity);
        return Task.FromResult<IActionResult>(Ok(portfolio));
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

    [HttpGet("gateway-operations/{accountId}")]
    public async Task<IActionResult> GetGatewayOperations(string accountId)
    {
        var operations = await _portfolioGateway.GetOperations(accountId);
        return Ok(operations);
    }

    [HttpPut("gateway-operations/replay/{accountId}")]
    public async Task<IActionResult> GetGatewayOperationsReplay(string accountId)
    {
        var portfolioRepo = CreateEmptyRepository(accountId);
        var service = new TradeOperationService(portfolioRepo, _staticMarketDataProvider);

        var operations = await _portfolioGateway.GetOperations(accountId);

        if (operations != null)
        {
            foreach (var operation in operations)
            {
                var portfolio = await service.ApplyOperation(operation);
                portfolioRepo.Save(portfolio);
            }
        }

        var portfolios = portfolioRepo.GetAllPortfolios();

        return Ok(portfolios);
    }

    private static IPortfolioRepository CreateEmptyRepository(string accountId)
    {
        var settings = new PortfolioSettings()
        {
            Accounts = [accountId]
        };

        var repo = new PortfolioRepository(Options.Create(settings));
        return repo;
    }
}
